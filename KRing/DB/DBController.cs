using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using KRing.DTO;
using System.Security.Cryptography;
using System.Security;

namespace KRing.DB
{
    /* We cant salt and store these passwords, since its just data, we are not trying to log a user in, we are trying to give them */
    /* Their specific decrypted data (which happens to be passwords) on demand once they are logged in */
    /* TODO: all strings, perhaps not username, in the below, should be encrypted. But we want I/O to work first */
    public class DBController
    {
        /// <summary>
        /// Singleton pattern, since we use the same file throughout, so we only want one accessing it at any time. 
        /// </summary>
        private static DBController _instance;
        private readonly string metaPath = "..\\..\\Data\\meta.txt";
        private readonly string configPath = "..\\..\\Data\\config.txt";
        private readonly string dbPath = "..\\..\\Data\\db.txt";
        private readonly byte[] _insecureHardcodedKey = Encoding.ASCII.GetBytes("Yellow Submarine");
        private byte[] _iv;

        public List<DBEntry> Entries { get; private set; }
        public int EntryCount { get; private set; }

        public static DBController Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new DBController();
                }

                return _instance;
            }
        }

        private DBController()
        {
            Entries = new List<DBEntry>();
            _iv = new byte[Authenticator.SaltByteSize];
            int count = 0;
            
            using (StreamReader sr = new StreamReader(configPath))
            {
                var readCount = sr.ReadLine();
                int.TryParse(readCount, out count);

                if (count > 0)
                {
                    using (FileStream fs = new FileStream(metaPath, FileMode.Open))
                    {
                        fs.Read(_iv, 0, Authenticator.SaltByteSize);
                    }
                }
                else _iv = Authenticator.GenerateSalt();
            }

            if (count > 0)
            {
                FileStream fs = new FileStream(dbPath, FileMode.Open);
                AesManaged aesManaged = new AesManaged();
                CryptoStream cs = new CryptoStream(fs, aesManaged.CreateDecryptor(_insecureHardcodedKey, _iv), CryptoStreamMode.Read);
                StreamReader streamReader = new StreamReader(cs);

                for(int i = 0; i < count; i++)
                {
                    var domain = streamReader.ReadLine();
                    var password = streamReader.ReadLine();

                    Console.WriteLine("domain: {0}, password: {1}", domain, password);

                    SecureString securePassword = new SecureString();
                    securePassword.PopulateWithString(password);
                    
                    DBEntry newEntry = new DBEntry(domain, securePassword);
                    Entries.Add(newEntry);
                    EntryCount++;
                }

                cs.Close();
                streamReader.Close();
                aesManaged.Dispose();
                fs.Close();
            }
            else
            {
                    EntryCount = 0;
            }
            
        }
        
        public void AddEntry(DBEntryDTO newDTO)
        {
            DBEntry newEntry = new DBEntry(newDTO.Domain, newDTO.Password);
            Entries.Add(newEntry);
            EntryCount++;
        }

        public void Write(string password)
        {
            using (StreamWriter configWriter = new StreamWriter(configPath))
            {
                configWriter.WriteLine(EntryCount);
            }

            using (FileStream fs = new FileStream(metaPath, FileMode.Create))
            {
                _iv = Authenticator.GenerateSalt();
                fs.Write(_iv, 0, Authenticator.SaltByteSize);
            }

            FileStream fileStream = new FileStream(dbPath, FileMode.Create);
            AesManaged aesManaged = new AesManaged();
            CryptoStream cs = new CryptoStream(fileStream, aesManaged.CreateEncryptor(_insecureHardcodedKey, _iv), CryptoStreamMode.Write);
            StreamWriter streamWriter = new StreamWriter(cs);

            foreach (var entr in Entries)
            {
                streamWriter.WriteLine(entr.Domain);
                streamWriter.WriteLine(entr.Password.ConvertToUnsecureString());
            }

            streamWriter.Close();
            cs.Close();
            aesManaged.Dispose();
            fileStream.Close();
        }
    }
}
