using System;
using System.Threading.Tasks;
using System.IO;
using KRingCore.Persistence.Interfaces;
using System.Text;
using KRingCore.Security;
using KRingCore.Persistence.Model;
using System.Collections.Generic;
using KRingCore.Interfaces;
using System.Security;
using KRingCore.Core.Model;

namespace KRingCore.Persistence.Repositories
{
    public class NsvStoredPasswordWriter : IStoredPasswordWriter
    {
        private readonly IDataConfig _config;

        private SymmetricKey _encrKey;
        private SymmetricKey _macKey;
        private readonly int _ivLength = 16;

        public NsvStoredPasswordWriter(SecureString password, SymmetricKey encrKey, SymmetricKey macKey, IDataConfig config)
        {
            _config = config;
            _encrKey = encrKey;
            _macKey = macKey;
        }

        public bool EncryptionErrorOccured { get; private set; }

        public void WriteEntriesToDb(List<StoredPassword> entries)
        {
            using (FileStream fileStream = new FileStream(_config.dbPath, FileMode.Create))
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                foreach (var entr in entries)
                {
                    try
                    {
                        var cipher = new AesHmacAuthenticatedCipher(System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);

                        /* encrypt */
                        var rawDomain = Encoding.UTF8.GetBytes(entr.Domain);
                        var rawUsername = Encoding.UTF8.GetBytes(entr.Username);
                        var rawPassword = entr.PlaintextPassword;
                        var rawPass = Encoding.UTF8.GetBytes(rawPassword);

                        var ivForDomain = CryptoHashing.GenerateSalt(_ivLength);
                        var ivForUsername = CryptoHashing.GenerateSalt(_ivLength);
                        var ivForPass = CryptoHashing.GenerateSalt(_ivLength);

                        var domainCipher = cipher.EncryptThenHMac(rawDomain, ivForDomain, _encrKey.Bytes, _macKey.Bytes);
                        var usernameCipher = cipher.EncryptThenHMac(rawUsername, ivForUsername, _encrKey.Bytes, _macKey.Bytes);
                        var passCipher = cipher.EncryptThenHMac(rawPass, ivForPass, _encrKey.Bytes, _macKey.Bytes);

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
                    catch (Exception)
                    {
                        EncryptionErrorOccured = true;
                    }
                }
            }
        }

        public async Task WriteEntriesToDbAsync(List<StoredPassword> entries)
        {
            using (FileStream fileStream = new FileStream(_config.dbPath, FileMode.Create))
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                foreach (var entr in entries)
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

                        var cipher = new AesHmacAuthenticatedCipher(System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);

                        var domainCipher = cipher.EncryptThenHMac(rawDomain, ivForDomain, _encrKey.Bytes, _macKey.Bytes);
                        var usernameCipher = cipher.EncryptThenHMac(rawUsername, ivForUsername, _encrKey.Bytes, _macKey.Bytes);
                        var passCipher = cipher.EncryptThenHMac(rawPass, ivForPass, _encrKey.Bytes, _macKey.Bytes);

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
                    catch (Exception)
                    {
                        EncryptionErrorOccured = true;
                    }
                }
            }
        }
    }
}
