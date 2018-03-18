using KRingCore.Core;
using KRingCore.Persistence.Model;
using System;
using System.Collections.Generic;
using KRingCore.Security;
using System.Linq;
using System.Security;
using System.Configuration;
using KRingCore.Persistence.Interfaces;
using KRingCore.Interfaces;
using System.Threading.Tasks;
using KRingCore.Core.Model;

namespace KRingCore.Persistence.Repositories
{
    /// <summary>
    /// TODO: only read from database in constructor, when called, and if changes have been made (i.e. we wrote) 
    /// </summary>
    public class StoredPasswordRepository : ReleasePathDependent, IStoredPasswordRepository
    {
        private readonly IDataConfig _dataConfig;
        private IStoredPasswordIO _passwordIO;

        private List<StoredPassword> _entries;

        private SecureString _password;

        private byte[] _saltForEncrKey;
        private byte[] _saltForMacKey;
        private SymmetricKey _encrKey;
        private SymmetricKey _macKey;

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
            _password = password;

            _encrKey = new SymmetricKey(password, _saltForEncrKey);
            _macKey = new SymmetricKey(password, _saltForMacKey);

            _passwordIO = new NsvStoredPasswordIO(password, _encrKey.Bytes, _macKey.Bytes, _dataConfig);

            _entries = LoadEntriesFromDb();  
        }

        /// <summary>
        /// Used for creating new repository at runtime, e.g. copy over password list at runtime. 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="encrKeySalt"></param>
        /// <param name="macKeySalt"></param>
        /// <param name="passwords"></param>
        public StoredPasswordRepository(SecureString password, byte[] encrKeySalt, byte[] macKeySalt, List<StoredPassword> passwords)
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
            _password = password;

            _encrKey = new SymmetricKey(password, _saltForEncrKey);
            _macKey = new SymmetricKey(password, _saltForMacKey);

            _passwordIO = new NsvStoredPasswordIO(password, _encrKey.Bytes, _macKey.Bytes, _dataConfig);

            _entries = passwords;
        }


        //TODO: do we need keys as objects? when and where to call dispose? when we clear keys? 
        public StoredPasswordRepository(SecureString password, SymmetricKey encrKey, SymmetricKey macKey, List<StoredPassword> passwords)
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

            _saltForEncrKey = new byte[0];
            _saltForMacKey = new byte[0];
            _password = password;

            _encrKey = encrKey;
            _macKey = macKey;

            _passwordIO = new NsvStoredPasswordIO(password, _encrKey.Bytes, _macKey.Bytes, _dataConfig);

            _entries = passwords;
        }

        public StoredPasswordRepository(SecureString password, byte[] encrKeySalt, byte[] macKeySalt, IDataConfig config)
        {
            _dataConfig = config;
            
            DecryptionErrorOccured = false;
            EncryptionErrorOccured = false;

            _saltForEncrKey = new byte[_ivLength];
            _saltForEncrKey = encrKeySalt;
            _saltForMacKey = macKeySalt;
            _password = password;

            _encrKey = new SymmetricKey(password, _saltForEncrKey);
            _macKey = new SymmetricKey(password, _saltForMacKey);

            _passwordIO = new NsvStoredPasswordIO(password, _encrKey.Bytes, _macKey.Bytes, _dataConfig);

            _entries = LoadEntriesFromDb();
        }

        //TODO: implement idisposable instead? We can use the idisposable of the new symmetrickeyclass.
        ~StoredPasswordRepository()
        {
            _encrKey.Dispose();
            _macKey.Dispose();
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
                entry.Username = updatedEntry.Username;
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

        public List<StoredPassword> ContainsSearch(string containsDomain)
        {
            return _entries.Where(e => e.Domain.Contains(containsDomain)).ToList();
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
            try
            {
                _passwordIO = new NsvStoredPasswordIO(_password, _encrKey.Bytes, _macKey.Bytes, _dataConfig);
                await _passwordIO.Writer.WriteEntriesToDbAsync(_entries);
            }
            catch (Exception)
            {
                EncryptionErrorOccured = _passwordIO.Writer.EncryptionErrorOccured;
            }
        }

        public void WriteEntriesToDb()
        {
            try
            {
                _passwordIO = new NsvStoredPasswordIO(_password, _encrKey.Bytes, _macKey.Bytes, _dataConfig);
                _passwordIO.Writer.WriteEntriesToDb(_entries);
            }
            catch(Exception)
            {
                EncryptionErrorOccured = _passwordIO.Writer.EncryptionErrorOccured;
            }                    
        }

        public async Task<List<StoredPassword>> LoadEntriesFromDbAsync()
        {
            try
            {
                _entries = await _passwordIO.Reader.LoadEntriesFromDbAsync();
                return _entries;
            }
            catch (Exception)
            {
                _entries = new List<StoredPassword>();
                throw new Exception("Could not load passwords - possibly data is corrupted!");
            }
            finally
            {
                DecryptionErrorOccured = _passwordIO.Reader.DecryptionErrorOccured;
            }
        } 

        public List<StoredPassword> LoadEntriesFromDb()
        {
            try
            {
                _entries = _passwordIO.Reader.LoadEntriesFromDb();
                return _entries;
            }
            catch(Exception)
            {
                _entries = new List<StoredPassword>();
                throw new Exception("Could not load passwords - possibly data is corrupted!");
            }
            finally
            {
                DecryptionErrorOccured = _passwordIO.Reader.DecryptionErrorOccured;
            }
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
