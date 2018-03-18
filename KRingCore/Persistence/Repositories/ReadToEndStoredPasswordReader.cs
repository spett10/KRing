using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KRingCore.Persistence.Interfaces;
using KRingCore.Persistence.Model;
using KRingCore.Core;
using KRingCore.Interfaces;
using System.Security;
using System.IO;
using System.Linq;
using KRingCore.Security;
using System.Text;
using KRingCore.Core.Model;

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
    public class ReadToEndStoredPasswordReader : ReleasePathDependent, IStoredPasswordReader
    {
        private readonly IDataConfig _config;
        private SymmetricKey _encrKey;
        private SymmetricKey _macKey;

        private const int _keyLength = 32;

        public bool DecryptionErrorOccured { get; private set; }

        public ReadToEndStoredPasswordReader(SecureString password, SymmetricKey encrKey, SymmetricKey macKey, IDataConfig config)
        {
            _config = config;

            DecryptionErrorOccured = false;
            _encrKey = encrKey;
            _macKey = macKey;
        }

        public List<StoredPassword> LoadEntriesFromDb()
        {
            var entries = new List<StoredPassword>();

            using (FileStream fileStream = new FileStream(_config.dbPath, FileMode.Open))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                try
                {
                    var contents = streamReader.ReadToEnd();

                    var loadedEntries = new NewLineSeperatedDb(contents, _encrKey, _macKey);

                    var entr = loadedEntries.ToList();

                    foreach(StoredPassword e in loadedEntries)
                    {
                        entries.Add(e);
                    }

                    DecryptionErrorOccured = false;
                }
                catch(Exception)
                {
                    DecryptionErrorOccured = true;
                }
            }

            return entries;
        }

        public async Task<List<StoredPassword>> LoadEntriesFromDbAsync()
        {
            var entries = new List<StoredPassword>();

            using (FileStream fileStream = new FileStream(_config.dbPath, FileMode.Open))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                try
                {
                    var contents = await streamReader.ReadToEndAsync();

                    var loadedEntries = new NewLineSeperatedDb(contents, _encrKey, _macKey);

                    var entr = loadedEntries.ToList();

                    foreach (StoredPassword e in loadedEntries)
                    {
                        entries.Add(e);
                    }

                    DecryptionErrorOccured = false;
                }
                catch (Exception)
                {
                    DecryptionErrorOccured = true;
                }
            }

            return entries;
        }
        
        /// <summary>
        /// TODO: More fitting name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class NewLineSeperatedDb : IEnumerable<StoredPassword>
        {
            private readonly List<StoredPassword> _entries;
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

            private SymmetricKey _encrKey;
            private SymmetricKey _macKey;

            public NewLineSeperatedDb(string db, SymmetricKey encrKey, SymmetricKey macKey)
            {
                var str = db.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                var count = str.Count();
                var _linesPerEntry = 9;
                var roundtrips = count / _linesPerEntry;

                _entries = new List<StoredPassword>();

                _encrKey = encrKey;
                _macKey = macKey;

                for(int i = 0; i < roundtrips; i++)
                {
                    var currentEntry = str.Skip(i * _linesPerEntry).Take(_linesPerEntry).ToArray();
                    _entries.Add(ParseEntry(currentEntry));
                }
            }

            private StoredPassword ParseEntry(string[] entry)
            {
                var domainBase64 = _extractDomain(entry);
                var domainTag = _extractDomainTag(entry);
                var domainIvBase64 = _extractDomainIv(entry);

                var usernameBase64 = _extractUsername(entry);
                var usernameTag = _extractUsernameTag(entry);
                var usernameIvBase64 = _extractUsernameIv(entry);

                var passwordBase64 = _extractPassword(entry);
                var passwordTag = _extractPasswordTag(entry);
                var passwordIvBase64 = _extractPasswordIv(entry);

                var cipher = new AesHmacAuthenticatedCipher(System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);

                var domainCipher = new AesHmacAuthenticatedCipher.AuthenticatedCiphertext(domainBase64, domainTag);
                var usernameCipher = new AesHmacAuthenticatedCipher.AuthenticatedCiphertext(usernameBase64, usernameTag);
                var passwordCipher = new AesHmacAuthenticatedCipher.AuthenticatedCiphertext(passwordBase64, passwordTag);

                var domainIv = Convert.FromBase64String(domainIvBase64);
                var usernameIv = Convert.FromBase64String(usernameIvBase64);
                var passwordIv = Convert.FromBase64String(passwordIvBase64);

                /* Decrypt and check tag. If tag is not valid, the used methods throws errors */
                var domain = cipher.VerifyMacThenDecrypt(domainCipher, _encrKey.Bytes, domainIv, _macKey.Bytes);
                var username = cipher.VerifyMacThenDecrypt(usernameCipher, _encrKey.Bytes, usernameIv, _macKey.Bytes);
                var password = cipher.VerifyMacThenDecrypt(passwordCipher, _encrKey.Bytes, passwordIv, _macKey.Bytes);

                return new StoredPassword(Encoding.UTF8.GetString(domain), Encoding.UTF8.GetString(username), Encoding.UTF8.GetString(password));
            }

            public IEnumerator<StoredPassword> GetEnumerator()
            {
                foreach(var stpswd in this._entries)
                {
                    yield return stpswd;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }

    
}
