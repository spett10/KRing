using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using KRing.Core;
using KRing.DTO;
using KRing.Extensions;
using KRing.Persistence.Model;

namespace KRing.Persistence.Controllers
{
    /* We cant salt and store these passwords, since its just data, we are not trying to log a user in, we are trying to give them */
    /* Their specific decrypted data (which happens to be passwords) on demand once they are logged in */
    /* TODO: use repository for file access, DB shouldnt access raw files? */
    public class DBController
    {
        /// <summary>
        /// Singleton pattern, since we use the same file throughout, so we only want one accessing it at any time. 
        /// </summary>
        private static DBController _instance;
        private readonly DataConfig _dataConfig;
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
            EntryCount = 0;
            _iv = new byte[CryptoHashing.SaltByteSize];
            _dataConfig = new DataConfig(
                               "..\\..\\Data\\meta.txt",
                               "..\\..\\Data\\db.txt",
                               "..\\..\\Data\\config.txt");
        }

        public void LoadDb()
        {
            int count = SetupConfig();

            if (count > 0)
            {
                FileStream fs = new FileStream(_dataConfig.dbPath, FileMode.Open);
                AesManaged aesManaged = new AesManaged();
                CryptoStream cs = new CryptoStream(
                                    fs, 
                                    aesManaged.CreateDecryptor(
                                        _insecureHardcodedKey, 
                                        _iv), 
                                    CryptoStreamMode.Read);

                StreamReader streamReader = new StreamReader(cs);

                for(int i = 0; i < count; i++)
                {
                    var domain = streamReader.ReadLine();
                    var password = streamReader.ReadLine();
                    
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

        public void DeleteEntryFromDomain(string domain)
        {
            var entry = Entries.SingleOrDefault(e => e.Domain.Equals(domain));
            if (entry != null)
            {
                Entries.Remove(entry);
                EntryCount--;
            }
            
        }

        public void AddEntry(DBEntryDTO newDto)
        {
            bool duplicateExists = Entries.Exists(
                                            e => e.
                                            Domain.
                                            Equals(newDto.Domain, StringComparison.OrdinalIgnoreCase));

            if (!duplicateExists)
            {
                DBEntry newEntry = new DBEntry(newDto.Domain, newDto.Password);
                Entries.Add(newEntry);
                EntryCount++;
            }
            else
            {
                throw new ArgumentException("Error: Domain Already Exists");
            }
        }

        public void UpdateEntry(DBEntryDTO updatedEntry)
        {
            var entry =
                Entries.FirstOrDefault(e => e.Domain.Equals(updatedEntry.Domain, StringComparison.OrdinalIgnoreCase));
            if (entry != null) entry.Password = updatedEntry.Password;
        }

        public bool ExistsEntry(string domain)
        {
            return Entries.Any(e => e.
                               Domain.
                               ToString().
                               Equals(domain, StringComparison.OrdinalIgnoreCase));
        }

        public SecureString GetPassword(string domain)
        {
            return Entries.Where(e => e.Domain == domain).Select(e => e.Password).First();
        }

        public void DeleteDb()
        {
            Entries.Clear();

            FileUtil.FilePurge(_dataConfig.dbPath, "-");
            FileUtil.FilePurge(_dataConfig.configPath, "0");
        }

        public void WriteDb(string password)
        {
            WriteCount();

            WriteIV();

            FileStream fileStream = new FileStream(_dataConfig.dbPath, FileMode.Create);
            AesManaged aesManaged = new AesManaged();
            CryptoStream cs = new CryptoStream(
                                fileStream, 
                                aesManaged.CreateEncryptor(
                                    _insecureHardcodedKey,
                                    _iv), 
                                CryptoStreamMode.Write);

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

        private int SetupConfig()
        {
            int count = 0;

            using (StreamReader sr = new StreamReader(_dataConfig.configPath))
            {
                var readCount = sr.ReadLine();
                if (readCount != null)
                {
                    int.TryParse(readCount, out count);

                    if (count > 0)
                    {
                        SetupIV();
                    }
                    else _iv = CryptoHashing.GenerateSalt();
                }
                else
                {
                    count = 0;
                    _iv = CryptoHashing.GenerateSalt();
                }
            }

            return count;
        }

        private void WriteIV()
        {
            using (FileStream fs = new FileStream(_dataConfig.metaPath, FileMode.Create))
            {
                _iv = CryptoHashing.GenerateSalt();
                fs.Write(_iv, 0, CryptoHashing.SaltByteSize);
            }
        }

        private void WriteCount()
        {
            using (StreamWriter configWriter = new StreamWriter(_dataConfig.configPath))
            {
                configWriter.WriteLine(EntryCount);
            }
        }

        private void SetupIV()
        {
            using (FileStream fs = new FileStream(_dataConfig.metaPath, FileMode.Open))
            {
                fs.Read(_iv, 0, CryptoHashing.SaltByteSize);
            }
        }
    }
}
