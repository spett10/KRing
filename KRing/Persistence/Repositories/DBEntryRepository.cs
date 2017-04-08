using KRing.Core;
using KRing.Extensions;
using KRing.Persistence.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Transactions;
using System.Configuration;
using KRing.Persistence.Interfaces;
using KRing.Interfaces;
using Security.Cryptography;
using System.Text;

namespace KRing.Persistence.Repositories
{
    public class DbEntryRepository : IDbEntryRepository
    {
        private readonly IDataConfig _dataConfig;
        private readonly int _count;
        private List<DBEntry> _entries;
        
        private byte[] _iv;
        private byte[] _key;

        private readonly int _keyLength = 32;
        private readonly int _ivLength = 12;

        private readonly SecureString _password;

        public int EntryCount => _entries.Count;
        public bool DecryptionErrorOccured { get; private set; }

        /// <summary>
        /// If you create a new Repository object, and the underlying DB already exists
        /// The password must be the same. Otherwise, the decryption will fail and an error must be thrown.
        /// </summary>
        /// <param name="password"></param>
        public DbEntryRepository(SecureString password)
        {
#if DEBUG
            _dataConfig = new DataConfig(
                               ConfigurationManager.AppSettings["relativemetaPathDebug"],
                               ConfigurationManager.AppSettings["relativedbPathDebug"],
                               ConfigurationManager.AppSettings["relativeconfigPathDebug"]);
#else
            _dataConfig = new DataConfig(
                               Environment.CurrentDirectory + ConfigurationManager.AppSettings["relativemetaPath"],
                               Environment.CurrentDirectory + ConfigurationManager.AppSettings["relativedbPath"],
                               Environment.CurrentDirectory + ConfigurationManager.AppSettings["relativeconfigPath"]);
#endif

            DecryptionErrorOccured = false;

            _count = _dataConfig.GetStorageCount();
            _iv = new byte[_ivLength];
            SetupMeta();

            _password = password;
            _key = DeriveKey(password);
            
            if(IsDbEmpty())
            {
                _entries = new List<DBEntry>();
                DeleteAllEntries();
            }
            else
            {
                _entries = LoadEntriesFromDb();
            }            
        }

        public DbEntryRepository(SecureString password, IDataConfig config)
        {
            _dataConfig = config;
            
            DecryptionErrorOccured = false;

            _count = _dataConfig.GetStorageCount();
            _iv = new byte[_ivLength];
            SetupMeta();

            _password = password;
            _key = DeriveKey(password);

            if (IsDbEmpty())
            {
                _entries = new List<DBEntry>();
                DeleteAllEntries();
            }
            else
            {
                _entries = LoadEntriesFromDb();
            }
        }

        public void DeleteEntry(string domain)
        {
            var entry = _entries.SingleOrDefault(e => e.Domain.Equals(domain));
            if (entry != null)
            {
                _entries.Remove(entry);
            }
            else
                throw new ArgumentException("domain does not exist to delete");
        }

        public void DeleteEntry(int index)
        {
            var entry = _entries.ElementAt(index);
            if (entry != null)
            {
                _entries.Remove(entry);
            }
            else
                throw new ArgumentException("domain does not exist to delete");
        }

        public void UpdateEntry(DBEntry updatedEntry)
        {
            var entry =
                _entries.FirstOrDefault(e => e.Domain.Equals(updatedEntry.Domain, StringComparison.OrdinalIgnoreCase));
            if (entry != null) entry.Password = updatedEntry.Password;
            else throw new ArgumentException("No such Domain");
        }
        
        public DBEntry GetEntry(int index)
        {
            return _entries.ElementAt(index);
        }

        public SecureString GetPasswordFromDomain(string domain)
        {
            return _entries.Where(e => e.Domain == domain).Select(e => e.Password).First();
        }

        public SecureString GetPasswordFromCount(int count)
        {
            return _entries.ElementAt(count).Password;
        }

        public void DeleteAllEntries()
        {
            _entries.Clear();

            DeleteDb();
        }
        
        public List<DBEntry> GetEntries()
        {
            return _entries;
        }

        public bool ExistsEntry(string domain)
        {
            return _entries.Any(e => e.
                               Domain.
                               ToString().
                               Equals(domain, StringComparison.CurrentCulture));
        }

        public void AddEntry(DBEntry newDbEntry)
        {
            bool duplicateExists = _entries.Exists(
                                            e => e.
                                            Domain.
                                            Equals(newDbEntry.Domain, StringComparison.CurrentCulture));

            if (!duplicateExists)
            {
                _entries.Add(newDbEntry);
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

        public void WriteEntriesToDb()
        {
            UpdateMeta();

            _key = DeriveKey(_password);

            FileStream fileStream = new FileStream(_dataConfig.dbPath, FileMode.Create);

            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                foreach (var entr in _entries)
                {
                    /* encrypt */
                    var rawDomain = Encoding.UTF8.GetBytes(entr.Domain);
                    var rawPass = Encoding.UTF8.GetBytes(entr.Password.ConvertToUnsecureString());

                    var ivForDomain = CryptoHashing.GenerateSalt(_ivLength);
                    var ivForPass = CryptoHashing.GenerateSalt(_ivLength);

                    var domainCipher = Aes256AuthenticatedCipher.Encrypt(rawDomain, _key, ivForDomain);
                    var passCipher = Aes256AuthenticatedCipher.Encrypt(rawPass, _key, ivForPass);

                    /* write domain, tag, iv */
                    streamWriter.WriteLine(domainCipher.GetCipherAsBase64());
                    streamWriter.WriteLine(domainCipher.GetTagAsBase64());
                    streamWriter.WriteLine(Convert.ToBase64String(ivForDomain));

                    /* write password, tag */
                    streamWriter.WriteLine(passCipher.GetCipherAsBase64());
                    streamWriter.WriteLine(passCipher.GetTagAsBase64());
                    streamWriter.WriteLine(Convert.ToBase64String(ivForPass));
                }
            }

            fileStream.Close();

            UpdateConfig(_entries.Count);

        }

        public List<DBEntry> LoadEntriesFromDb()
        {
            try
            {
                List<DBEntry> entries = new List<DBEntry>();

                FileStream fs = new FileStream(_dataConfig.dbPath, FileMode.Open);

                StreamReader streamReader = new StreamReader(fs);

                for (int i = 0; i < _count; i++)
                {
                    /* READ */
                    var domainBase64 = streamReader.ReadLine();
                    var domainTagBase64 = streamReader.ReadLine();
                    var domainIv = Convert.FromBase64String(streamReader.ReadLine());

                    var domainCipher = new Aes256AuthenticatedCipher.AuthenticatedCiphertext(domainBase64, domainTagBase64);

                    var passwordBase64 = streamReader.ReadLine();
                    var passwordTag = streamReader.ReadLine();
                    var passwordIv = Convert.FromBase64String(streamReader.ReadLine());

                    var passwordCipher = new Aes256AuthenticatedCipher.AuthenticatedCiphertext(passwordBase64, passwordTag);

                    try
                    {
                        /* DECRYPT */
                        var domain = Aes256AuthenticatedCipher.Decrypt(domainCipher, _key, domainIv);
                        var password = Aes256AuthenticatedCipher.Decrypt(passwordCipher, _key, passwordIv);

                        /* CREATE DBENTRY */
                        SecureString securePassword = new SecureString();
                        securePassword.PopulateWithString(Encoding.UTF8.GetString(password));

                        DBEntry newEntry = new DBEntry(Encoding.UTF8.GetString(domain), securePassword);
                        entries.Add(newEntry);
                    }
                    catch(Exception)
                    {
                        DecryptionErrorOccured = true;
                    }
                    
                }

                fs.Close();

                return entries;
            }
            catch(Exception)
            {
                throw new Exception("Could not load passwords - possibly data is corrupted!");
            }
        }
        
        private void UpdateMeta()
        {
            using (FileStream fs = new FileStream(_dataConfig.metaPath, FileMode.Create))
            {
                _iv = CryptoHashing.GenerateSalt(_ivLength);
                fs.Write(_iv, 0, _iv.Length);
            }
        }

        private void SetupMeta()
        {
            if (_count > 0)
            {
                using (FileStream fs = new FileStream(_dataConfig.metaPath, FileMode.Open))
                {
                    fs.Read(_iv, 0, _iv.Length);
                }
            }
            else
            {
                _iv = CryptoHashing.GenerateSalt(_ivLength);
            }
        }

        private byte[] DeriveKey(SecureString password)
        {
            return CryptoHashing.DeriveKeyFromPasswordAndSalt(password, _iv, _keyLength);
        }

        private void DeleteDb()
        {
            FileUtil.FilePurge(_dataConfig.dbPath, "-");
            _dataConfig.ClearConfig();
        }

        private void UpdateConfig(int count)
        {
            _dataConfig.UpdateConfig(count);
        }
    }
}
