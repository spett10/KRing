using KRingCore.Core;
using KRingCore.Extensions;
using KRingCore.Persistence.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Configuration;
using KRingCore.Persistence.Interfaces;
using KRingCore.Persistence;
using KRingCore.Interfaces;
using System.Text;
using System.Threading.Tasks;

namespace KRingCore.Persistence.Repositories
{
    /// <summary>
    /// TODO: only read from database in constructor, when called, and if changes have been made (i.e. we wrote) 
    /// </summary>
    public class StoredPasswordRepository : ReleasePathDependent, IStoredPasswordRepository
    {
        private readonly IDataConfig _dataConfig;
        private readonly IStoredPasswordReader _passwordReader;

        private List<StoredPassword> _entries;
        
        private byte[] _saltForEncrKey;
        private byte[] _saltForMacKey;
        private byte[] _encrKey;
        private byte[] _macKey;

        private readonly int _keyLength = 32;
        private readonly int _ivLength = 16;

        public int EntryCount => _entries.Count;
        public bool DecryptionErrorOccured { get; private set; }
        public bool EncryptionErrorOccured { get; private set; }

        /// <summary>
        /// If you create a new Repository object, and the underlying DB already exists
        /// The password must be the same. Otherwise, the decryption will fail and an error must be thrown.
        /// </summary>
        /// <param name="password"></param>
        public StoredPasswordRepository(SecureString password, byte[] encrKeySalt, byte[] macKeySalt)
        {
#if DEBUG
            _dataConfig = new DataConfig(
                               ConfigurationManager.AppSettings["relativedbPathDebug"]);
#else
            _dataConfig = new DataConfig(
                               base.ReleasePathPrefix() + ConfigurationManager.AppSettings["relativedbPath"]);
#endif

            DecryptionErrorOccured = false;
            EncryptionErrorOccured = false;

            _saltForEncrKey = encrKeySalt;
            _saltForMacKey = macKeySalt;

            _encrKey = DeriveKey(password, _saltForEncrKey);
            _macKey = DeriveKey(password, _saltForMacKey);

            _passwordReader = new ReadToEndStoredPasswordReader(password, _encrKey, _macKey);

            _entries = LoadEntriesFromDb();  
        }

        public StoredPasswordRepository(SecureString password, byte[] encrKeySalt, byte[] macKeySalt, IDataConfig config)
        {
            _dataConfig = config;
            
            DecryptionErrorOccured = false;
            EncryptionErrorOccured = false;

            _saltForEncrKey = new byte[_ivLength];
            _saltForEncrKey = encrKeySalt;
            _saltForMacKey = macKeySalt;

            _encrKey = DeriveKey(password, _saltForEncrKey);
            _macKey = DeriveKey(password, _saltForMacKey);

            _passwordReader = new ReadToEndStoredPasswordReader(password, _encrKey, _macKey);

            _entries = LoadEntriesFromDb();
        }

        ~StoredPasswordRepository()
        {
            CryptoHashing.ZeroOutArray(ref _encrKey);
            CryptoHashing.ZeroOutArray(ref _macKey);
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

        public void UpdateEntry(StoredPassword updatedEntry)
        {
            var entry =
                _entries.FirstOrDefault(e => e.Domain.Equals(updatedEntry.Domain, StringComparison.OrdinalIgnoreCase));
            if (entry != null)
            {
                entry.PlaintextPassword = updatedEntry.PlaintextPassword;
            }
            else throw new ArgumentException("No such Domain");
        }
        
        public StoredPassword GetEntry(int index)
        {
            return _entries.ElementAt(index);
        }

        public string GetPasswordFromDomain(string domain)
        {
            return _entries.Where(e => e.Domain == domain).Select(e => e.PlaintextPassword).First();
        }

        public string GetPasswordFromCount(int count)
        {
            return _entries.ElementAt(count).PlaintextPassword;
        }

        public void DeleteAllEntries()
        {
            _entries.Clear();

            DeleteDb();
        }
        

        public List<StoredPassword> GetEntries()
        {
            return _entries;
        }

        public List<StoredPassword> PrefixSearch(string prefixDomain)
        {
            return _entries.Where(e => e.Domain.StartsWith(prefixDomain)).ToList();
        }

        public bool ExistsEntry(string domain)
        {
            return _entries.Any(e => e.
                               Domain.
                               ToString().
                               Equals(domain, StringComparison.CurrentCulture));
        }

        public void AddEntry(StoredPassword newDbEntry)
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
            return _entries.Count() <= 0;
        } 

        public async Task WriteEntriesToDbAsync()
        {
            using (FileStream fileStream = new FileStream(_dataConfig.dbPath, FileMode.Create))
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                foreach(var entr in _entries)
                {
                    try
                    {
                        /* Encrypt */
                        var rawDomain = Encoding.UTF8.GetBytes(entr.Domain);
                        var rawUsername = Encoding.UTF8.GetBytes(entr.Username);
                        var rawPassword = entr.PlaintextPassword;
                        var rawPass = Encoding.UTF8.GetBytes(rawPassword);

                        var ivForDomain = CryptoHashing.GenerateSalt(_ivLength);
                        var ivForUsername = CryptoHashing.GenerateSalt(_ivLength);
                        var ivForPass = CryptoHashing.GenerateSalt(_ivLength);

                        var domainCipher = Aes256AuthenticatedCipher.CBCEncryptThenHMac(rawDomain, ivForDomain, _encrKey, _macKey);
                        var usernameCipher = Aes256AuthenticatedCipher.CBCEncryptThenHMac(rawUsername, ivForUsername, _encrKey, _macKey);
                        var passCipher = Aes256AuthenticatedCipher.CBCEncryptThenHMac(rawPass, ivForPass, _encrKey, _macKey);

                        /* write domain, tag, iv */
                        await streamWriter.WriteLineAsync(domainCipher.GetCipherAsBase64());
                        await streamWriter.WriteLineAsync(domainCipher.GetTagAsBase64());
                        await streamWriter.WriteLineAsync(Convert.ToBase64String(ivForDomain));

                        /* write username, tag, iv */
                        await streamWriter.WriteLineAsync(usernameCipher.GetCipherAsBase64());
                        await streamWriter.WriteLineAsync(usernameCipher.GetTagAsBase64());
                        await streamWriter.WriteLineAsync(Convert.ToBase64String(ivForUsername));

                        /* write password, tag, iv */
                        await streamWriter.WriteLineAsync(passCipher.GetCipherAsBase64());
                        await streamWriter.WriteLineAsync(passCipher.GetTagAsBase64());
                        await streamWriter.WriteLineAsync(Convert.ToBase64String(ivForPass));

                        EncryptionErrorOccured = false;                        
                    }
                    catch(Exception)
                    {
                        EncryptionErrorOccured = true;
                    }
                }
            }            
        }

        public void WriteEntriesToDb()
        {
            using(FileStream fileStream = new FileStream(_dataConfig.dbPath, FileMode.Create))
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                foreach (var entr in _entries)
                {
                    try
                    {
                        /* encrypt */
                        var rawDomain = Encoding.UTF8.GetBytes(entr.Domain);
                        var rawUsername = Encoding.UTF8.GetBytes(entr.Username);
                        var rawPassword = entr.PlaintextPassword;
                        var rawPass = Encoding.UTF8.GetBytes(rawPassword);

                        var ivForDomain = CryptoHashing.GenerateSalt(_ivLength);
                        var ivForUsername = CryptoHashing.GenerateSalt(_ivLength);
                        var ivForPass = CryptoHashing.GenerateSalt(_ivLength);

                        var domainCipher = Aes256AuthenticatedCipher.CBCEncryptThenHMac(rawDomain, ivForDomain, _encrKey, _macKey);
                        var usernameCipher = Aes256AuthenticatedCipher.CBCEncryptThenHMac(rawUsername, ivForUsername, _encrKey, _macKey);
                        var passCipher = Aes256AuthenticatedCipher.CBCEncryptThenHMac(rawPass, ivForPass, _encrKey, _macKey);

                        /* write domain, tag, iv */
                        streamWriter.WriteLine(domainCipher.GetCipherAsBase64());
                        streamWriter.WriteLine(domainCipher.GetTagAsBase64());
                        streamWriter.WriteLine(Convert.ToBase64String(ivForDomain));

                        /* write username, tag, iv */
                        streamWriter.WriteLine(usernameCipher.GetCipherAsBase64());
                        streamWriter.WriteLine(usernameCipher.GetTagAsBase64());
                        streamWriter.WriteLine(Convert.ToBase64String(ivForUsername));

                        /* write password, tag */
                        streamWriter.WriteLine(passCipher.GetCipherAsBase64());
                        streamWriter.WriteLine(passCipher.GetTagAsBase64());
                        streamWriter.WriteLine(Convert.ToBase64String(ivForPass));

                        EncryptionErrorOccured = false;
                    }
                    catch(Exception)
                    {
                        EncryptionErrorOccured = true;
                    }                    
                }
            }
        }

        public async Task<List<StoredPassword>> LoadEntriesFromDbAsync()
        {
            try
            {
                _entries = await _passwordReader.LoadEntriesFromDbAsync();
                return _entries;
            }
            catch (Exception)
            {
                _entries = new List<StoredPassword>();
                throw new Exception("Could not load passwords - possibly data is corrupted!");
            }
            finally
            {
                DecryptionErrorOccured = _passwordReader.DecryptionErrorOccured;
            }
        } 

        public List<StoredPassword> LoadEntriesFromDb()
        {
            try
            {
                _entries = _passwordReader.LoadEntriesFromDb();
                return _entries;
            }
            catch(Exception)
            {
                _entries = new List<StoredPassword>();
                throw new Exception("Could not load passwords - possibly data is corrupted!");
            }
            finally
            {
                DecryptionErrorOccured = _passwordReader.DecryptionErrorOccured;
            }
        }
        
        private byte[] DeriveKey(SecureString password, byte[] iv)
        {
            return CryptoHashing.DeriveKeyFromPasswordAndSalt(password, iv, _keyLength);
        }

        private void DeleteDb()
        {
            FileUtil.FilePurge(_dataConfig.dbPath, "-");
        }

        private async Task DeleteDbAsync()
        {
            await FileUtil.FilePurgeAsync(_dataConfig.dbPath, "-");
        }

        public StoredPassword GetEntry(string domain)
        {
            return _entries.Where(e => e.Domain == domain).FirstOrDefault();
        }
    }
}
