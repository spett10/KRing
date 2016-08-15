using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using KRing.Core;
using KRing.Core.Model;
using KRing.Extensions;
using KRing.Persistence.Model;

namespace KRing.Persistence.Repositories
{
    public class DbEntryRepository
    {
        private readonly DataConfig _dataConfig;
        private readonly byte[] _insecureHardcodedKey = Encoding.ASCII.GetBytes("Yellow Submarine");
        private byte[] _iv;
        private readonly int _count;
        
        public DbEntryRepository()
        {
            _dataConfig = new DataConfig(
                               "..\\..\\Data\\meta.txt",
                               "..\\..\\Data\\db.txt",
                               "..\\..\\Data\\config.txt");

            _count = Config();
            _iv = new byte[CryptoHashing.SaltByteSize];
            SetupIv();
        }

        public bool IsDbEmpty()
        {
            return _count > 0;
        }

        public void DeleteDb()
        {
            FileUtil.FilePurge(_dataConfig.dbPath, "-");
            FileUtil.FilePurge(_dataConfig.configPath, "0");
        }

        public void UpdateConfig(int count)
        {
            using (StreamWriter configWriter = new StreamWriter(_dataConfig.configPath))
            {
                configWriter.WriteLine(count);
            }
        }

        public void WriteEntriesToDb(List<DBEntry> entries)
        {
            UpdateConfig(entries.Count);

            UpdateIv();

            FileStream fileStream = new FileStream(_dataConfig.dbPath, FileMode.Create);
            AesManaged aesManaged = new AesManaged();
            CryptoStream cs = new CryptoStream(
                                fileStream,
                                aesManaged.CreateEncryptor(
                                    _insecureHardcodedKey,
                                    _iv),
                                CryptoStreamMode.Write);

            StreamWriter streamWriter = new StreamWriter(cs);

            foreach (var entr in entries)
            {
                streamWriter.WriteLine(entr.Domain);
                streamWriter.WriteLine(entr.Password.ConvertToUnsecureString());
            }

            streamWriter.Close();
            cs.Close();
            aesManaged.Dispose();
            fileStream.Close();
        }

        public List<DBEntry> LoadEntriesFromDb()
        {
            List<DBEntry> entries = new List<DBEntry>();

            FileStream fs = new FileStream(_dataConfig.dbPath, FileMode.Open);
            AesManaged aesManaged = new AesManaged();
            CryptoStream cs = new CryptoStream(
                                fs,
                                aesManaged.CreateDecryptor(
                                    _insecureHardcodedKey,
                                    _iv),
                                CryptoStreamMode.Read);

            StreamReader streamReader = new StreamReader(cs);

            for (int i = 0; i < _count; i++)
            {
                var domain = streamReader.ReadLine();
                var password = streamReader.ReadLine();

                SecureString securePassword = new SecureString();
                securePassword.PopulateWithString(password);

                DBEntry newEntry = new DBEntry(domain, securePassword);
                entries.Add(newEntry);
            }

            cs.Close();
            streamReader.Close();
            aesManaged.Dispose();
            fs.Close();

            return entries;
        }

        private int Config()
        {
            int count = 0;

            using (StreamReader sr = new StreamReader(_dataConfig.configPath))
            {
                var readCount = sr.ReadLine();
                if (readCount != null)
                {
                    int.TryParse(readCount, out count);
                }
                else
                {
                    count = 0;
                }
            }

            return count;
        }


        private void UpdateIv()
        {
            using (FileStream fs = new FileStream(_dataConfig.metaPath, FileMode.Create))
            {
                _iv = CryptoHashing.GenerateSalt();
                fs.Write(_iv, 0, CryptoHashing.SaltByteSize);
            }
        }

        private void SetupIv()
        {
            if (_count > 0)
            {
                using (FileStream fs = new FileStream(_dataConfig.metaPath, FileMode.Open))
                {
                    fs.Read(_iv, 0, CryptoHashing.SaltByteSize);
                }
            }
            else
            {
                _iv = CryptoHashing.GenerateSalt();
            }
        }
    }
}
