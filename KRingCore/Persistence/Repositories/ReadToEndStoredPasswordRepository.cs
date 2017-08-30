using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KRingCore.Persistence.Interfaces;
using KRingCore.Persistence.Model;
using KRingCore.Core;
using KRingCore.Interfaces;
using System.Security;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Collections;
using System.Text;

namespace KRingCore.Persistence.Repositories
{
    /// <summary>
    /// TODO: Maybe this should not be entire IStoredPasswordRepository, since its only the read part we
    /// are looking to make different (so we dont need config). 
    /// 
    /// So perhaps a IStoredPasswordReader interface, and then we inject this into the normal storedpasswordrepository,
    /// and also have an implementation that is the "old" way of just reading them. no wait, delete the old way
    /// when this works, we dont want the config file i guess.
    /// </summary>
    public class ReadToEndStoredPasswordRepository : ReleasePathDependent, IStoredPasswordRepository
    {
        private readonly string _dbPath;

        private readonly int _count;
        private List<StoredPassword> _entries;

        private byte[] _saltForEncrKey;
        private byte[] _saltForMacKey;
        private byte[] _encrKey;
        private byte[] _macKey;

        private const int _keyLength = 32;

        public int EntryCount => throw new NotImplementedException();

        public bool DecryptionErrorOccured { get; private set; }

        public bool EncryptionErrorOccured { get; private set; }

        public ReadToEndStoredPasswordRepository(SecureString password, byte[] encrKeySalt, byte[] macKeySalt)
        {
#if DEBUG
            _dbPath = ConfigurationManager.AppSettings["relativedbPathDebug"];
#else
            _dbPath = base.ReleasePathPrefix() + ConfigurationManager.AppSettings["relativedbPath"];
#endif
            DecryptionErrorOccured = false;
            EncryptionErrorOccured = false;

            _count = 0;

            _saltForEncrKey = encrKeySalt;
            _saltForMacKey = macKeySalt;

            _encrKey = CryptoHashing.DeriveKeyFromPasswordAndSalt(password, _saltForEncrKey, _keyLength);
            _macKey = CryptoHashing.DeriveKeyFromPasswordAndSalt(password, _saltForMacKey, _keyLength);

            _entries = LoadEntriesFromDb();
        }

        ~ReadToEndStoredPasswordRepository()
        {
            CryptoHashing.ZeroOutArray(ref _encrKey);
            CryptoHashing.ZeroOutArray(ref _macKey);
        }

        public void AddEntry(StoredPassword newDbEntry)
        {
            throw new NotImplementedException();
        }

        public void DeleteAllEntries()
        {
            throw new NotImplementedException();
        }

        public void DeleteEntry(string domain)
        {
            throw new NotImplementedException();
        }

        public void DeleteEntry(int index)
        {
            throw new NotImplementedException();
        }

        public bool ExistsEntry(string domain)
        {
            throw new NotImplementedException();
        }

        public List<StoredPassword> GetEntries()
        {
            throw new NotImplementedException();
        }

        public StoredPassword GetEntry(int index)
        {
            throw new NotImplementedException();
        }

        public StoredPassword GetEntry(string domain)
        {
            throw new NotImplementedException();
        }

        public string GetPasswordFromCount(int count)
        {
            throw new NotImplementedException();
        }

        public string GetPasswordFromDomain(string domain)
        {
            throw new NotImplementedException();
        }

        public bool IsDbEmpty()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO: Make a container class that has the entire string inside it, and make it implement
        /// IEnumerable - and it could return a single object per entry with all the fields readily available?
        /// Then you could simply read the string, create the object, and then do a foreach, extract the properties and insert into the passwordlist. 
        /// </summary>
        /// <returns></returns>
        public List<StoredPassword> LoadEntriesFromDb()
        {
            var entries = new List<StoredPassword>();

            using (FileStream fileStream = new FileStream(_dbPath, FileMode.Open))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                try
                {
                    var contents = streamReader.ReadToEnd();

                    var rawEntries = new InMemoryDatabase<StoredPassword>(contents, _encrKey, _macKey);

                    foreach(StoredPassword e in rawEntries)
                    {
                        _entries.Add(e);
                    }

                    DecryptionErrorOccured = false;
                }
                catch
                {
                    DecryptionErrorOccured = true;
                }
            }

            return entries;
        }

        public Task<List<StoredPassword>> LoadEntriesFromDbAsync()
        {
            throw new NotImplementedException();
        }

        public List<StoredPassword> PrefixSearch(string prefixDomain)
        {
            throw new NotImplementedException();
        }

        public void UpdateEntry(StoredPassword updatedEntry)
        {
            throw new NotImplementedException();
        }

        public void WriteEntriesToDb()
        {
            throw new NotImplementedException();
        }

        public Task WriteEntriesToDbAsync()
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// TODO: More fitting name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class InMemoryDatabase<T> : IEnumerable
        {
            private readonly List<string[]> _dbRaw;
            private const int _linesPerEntry = 9;

            private delegate string ExtractTokenFromEntry(string[] array);
            private ExtractTokenFromEntry _extractDomain = a => { return a[0]; };
            private ExtractTokenFromEntry _extractDomainTag = a => { return a[1]; };
            private ExtractTokenFromEntry _extractDomainIv = a => { return a[2]; };
            private ExtractTokenFromEntry _extractUsername = a => { return a[3]; };
            private ExtractTokenFromEntry _extractUsernameTag = a => { return a[4]; };
            private ExtractTokenFromEntry _extractUsernameIv = a => { return a[5]; };
            private ExtractTokenFromEntry _extractPassword = a => { return a[6]; };
            private ExtractTokenFromEntry _extractPasswordTag = a => { return a[7]; };
            private ExtractTokenFromEntry _extractPasswordIv = a => { return a[8]; };

            private int _index;

            private byte[] _encrKey;
            private byte[] _macKey;

            public InMemoryDatabase(string db, byte[] encrKey, byte[] macKey)
            {
                var str = db.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                var count = str.Count();
                var _linesPerEntry = 9;
                var roundtrips = count / _linesPerEntry;

                _dbRaw = new List<string[]>();

                _encrKey = encrKey;
                _macKey = macKey;

                for(int i = 0; i < roundtrips; i++)
                {
                    var currentEntry = str.Skip(i * _linesPerEntry).Take(_linesPerEntry).ToArray();
                    _dbRaw.Add(currentEntry);
                }

                _index = 0;
            }

            ~InMemoryDatabase()
            {
                CryptoHashing.ZeroOutArray(ref _encrKey);
                CryptoHashing.ZeroOutArray(ref _macKey);
            }

            public IEnumerator GetEnumerator()
            {
                var entry = _dbRaw[_index];

                _index++;

                var domainBase64 = _extractDomain(entry);
                var domainTag = _extractDomainTag(entry);
                var domainIvBase64 = _extractDomainIv(entry);

                var usernameBase64 = _extractUsername(entry);
                var usernameTag = _extractUsernameTag(entry);
                var usernameIvBase64 = _extractUsernameIv(entry);

                var passwordBase64 = _extractPassword(entry);
                var passwordTag = _extractPasswordTag(entry);
                var passwordIvBase64 = _extractPasswordIv(entry);

                var domainCipher = new Aes256AuthenticatedCipher.AuthenticatedCiphertext(domainBase64, domainTag);
                var usernameCipher = new Aes256AuthenticatedCipher.AuthenticatedCiphertext(usernameBase64, usernameTag);
                var passwordCipher = new Aes256AuthenticatedCipher.AuthenticatedCiphertext(passwordBase64, passwordTag);

                var domainIv = Convert.FromBase64String(domainIvBase64);
                var usernameIv = Convert.FromBase64String(usernameIvBase64);
                var passwordIv = Convert.FromBase64String(passwordIvBase64);

                /* Decrypt and check tag. If tag is not valid, the used methods throws errors */
                var domain = Aes256AuthenticatedCipher.VerifyMacThenCBCDecrypt(domainCipher, _encrKey, domainIv, _macKey);
                var username = Aes256AuthenticatedCipher.VerifyMacThenCBCDecrypt(usernameCipher, _encrKey, usernameIv, _macKey);
                var password = Aes256AuthenticatedCipher.VerifyMacThenCBCDecrypt(passwordCipher, _encrKey, passwordIv, _macKey);

                yield return new StoredPassword(Encoding.UTF8.GetString(domain), Encoding.UTF8.GetString(username), Encoding.UTF8.GetString(password));
            }
        }
    }

    
}
