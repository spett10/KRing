using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using KRing.DTO;
using System.Security.Cryptography;

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
        private static DBController instance;

        private readonly string DBPath = "..\\..\\Data\\db.txt";
        private readonly string INSECURE_HARDCODED_KEY = "Yellow Submarine";


        public List<DBEntry> Entries { get; private set; }
        public int EntryCount { get; private set; }

        public static DBController Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new DBController();
                }

                return instance;
            }
        }

        private DBController()
        {
            Entries = new List<DBEntry>();
            using (StreamReader sr = new StreamReader(DBPath))
            {
                int count = 0;
                bool IsDBPopulated = Int32.TryParse(sr.ReadLine(), out count);

                if(IsDBPopulated)
                {
                    EntryCount = count;

                    for (int i = 0; i < EntryCount; i++)
                    {
                        string username = sr.ReadLine();
                        string domain = sr.ReadLine();
                        string password = sr.ReadLine();
                        string salt = sr.ReadLine();

                        DBEntry newEntry = new DBEntry(domain, password);

                        Entries.Add(newEntry);
                    }
                }
                else
                {
                    EntryCount = 0;
                }
            }

        }
        

        public void AddEntry(DBEntryDTO newDTO)
        {
            DBEntry newEntry = new DBEntry(newDTO.Domain, newDTO.Password.ConvertToUnsecureString());
            Entries.Add(newEntry);
            EntryCount++;
        }
        
        /* we could call write after addentry? */
        public void Write(string password)
        {
            //FileStream fs = new FileStream(DBPath, FileMode.Create);

            /* Derive key from user passsword */
            /* but where do we store the key? */
            //byte[] key_salt = Authenticator.GenerateSalt();
            //Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, key_salt, 4096);             
            //int keysize_in_bytes = 16;
            //byte[] raw_key = rfc.GetBytes(keysize_in_bytes);

            /* Are we sure these sizes are correct? */
            byte[] raw_key = Encoding.ASCII.GetBytes(INSECURE_HARDCODED_KEY);
            byte[] IV = Authenticator.GenerateSalt();

            /* First, write count at top of DB. Then write IV, neither of these should be encrypted? */
            using (StreamWriter sw = new StreamWriter(DBPath, false))
            {
                sw.WriteLine(EntryCount);
                sw.WriteLine(Encoding.ASCII.GetString(IV));
            }

            using (FileStream fs = new FileStream(DBPath, FileMode.Append))
            {
                using (RijndaelManaged RMcrypto = new RijndaelManaged())
                {
                    using (CryptoStream cs = new CryptoStream(fs, RMcrypto.CreateEncryptor(raw_key, IV), CryptoStreamMode.Write))
                    {
                        using (StreamWriter encr = new StreamWriter(cs))
                        {
                            foreach(var entry in Entries)
                            {
                                List<string> formatedEnty = entry.ToStrings();
                                foreach(var str in formatedEnty)
                                {
                                    encr.WriteLine(str);
                                }
                            }
                        }
                    }
                }
            }

            
        }
    }
}
