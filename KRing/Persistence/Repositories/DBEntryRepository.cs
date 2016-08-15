using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using KRing.Core;
using KRing.Core.Model;
using KRing.DTO;
using KRing.Extensions;
using KRing.Interfaces;
using KRing.Persistence.Model;

namespace KRing.Persistence.Repositories
{
    public class DbEntryRepository
    {
        private readonly DataConfig _dataConfig;
        private readonly byte[] _insecureHardcodedKey = Encoding.ASCII.GetBytes("Yellow Submarine");
        private byte[] _iv;
        private readonly int _count;
        private readonly List<DBEntry> _entries;

        public int EntryCount => _entries.Count;

        public DbEntryRepository()
        {
            _dataConfig = new DataConfig(
                               "..\\..\\Data\\meta.txt",
                               "..\\..\\Data\\db.txt",
                               "..\\..\\Data\\config.txt");

            _count = Config();
            _iv = new byte[CryptoHashing.SaltByteSize];
            SetupIv();
            _entries = !IsDbEmpty() ? LoadEntriesFromDb() : new List<DBEntry>();
        }

        public void DeleteEntry(string domain)
        {
            var entry = _entries.SingleOrDefault(e => e.Domain.Equals(domain));
            if (entry != null)
            {
                _entries.Remove(entry);
            }
        }

        public void UpdateEntry(DbEntryDto updatedEntry)
        {
            var entry =
                _entries.FirstOrDefault(e => e.Domain.Equals(updatedEntry.Domain, StringComparison.OrdinalIgnoreCase));
            if (entry != null) entry.Password = updatedEntry.Password;
        }

        public SecureString GetPasswordFromEntry(string domain)
        {
            return _entries.Where(e => e.Domain == domain).Select(e => e.Password).First();
        }

        public void DeleteAllEntries()
        {
            _entries.Clear();

            DeleteDb();
        }

        public void ShowAllDomainsToUser(IUserInterface ui)
        {
            ui.MessageToUser("Stored Domains:");

            foreach (var entr in _entries)
            {
                ui.MessageToUser(entr.Domain);
            }
        }

        public bool ExistsEntry(string domain)
        {
            return _entries.Any(e => e.
                               Domain.
                               ToString().
                               Equals(domain, StringComparison.CurrentCulture));
        }

        public void AddEntry(DbEntryDto newDto)
        {
            bool duplicateExists = _entries.Exists(
                                            e => e.
                                            Domain.
                                            Equals(newDto.Domain, StringComparison.CurrentCulture));

            if (!duplicateExists)
            {
                DBEntry newEntry = new DBEntry(newDto.Domain, newDto.Password);
                _entries.Add(newEntry);
            }
            else
            {
                throw new ArgumentException("Error: Domain Already Exists");
            }
        }

        public bool IsDbEmpty()
        {
            return _count <= 0;
        }

        private void DeleteDb()
        {
            FileUtil.FilePurge(_dataConfig.dbPath, "-");
            FileUtil.FilePurge(_dataConfig.configPath, "0");
        }

        private void UpdateConfig(int count)
        {
            using (StreamWriter configWriter = new StreamWriter(_dataConfig.configPath))
            {
                configWriter.WriteLine(count);
            }
        }

        public void WriteEntriesToDb()
        {
            UpdateConfig(_entries.Count);

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

            foreach (var entr in _entries)
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
