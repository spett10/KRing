using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KRingCore.Persistence.Model;
using System.IO;
using Newtonsoft.Json;
using KRingCore.Core.Model;
using KRingCore.Extensions;
using System.Security;
using System.Text;
using KRingCore.Security;
using KRingCore.Persistence.Interfaces;
using System.Security.Cryptography;

namespace KRingCore.Core.Services
{
    public class DecryptingPasswordImporter : IDecryptingPasswordImporter
    {
        public List<StoredPassword> ImportPasswords(string filename, SecureString password, IStreamReadToEnd streamReader)
        {
            if (String.IsNullOrEmpty(filename) || password == null)
            {
                throw new ArgumentException("Invalid argument");
            }

            try
            {
                var contents = streamReader.ReadToEnd(filename);

                if (String.IsNullOrEmpty(contents))
                {
                    return new List<StoredPassword>();
                }

                ExportedEncryptedPasswords passwords = JsonConvert.DeserializeObject<ExportedEncryptedPasswords>(contents);

                var iterations = Configuration.ExportImportIterations;
                var raw = Encoding.UTF8.GetBytes(password.ConvertToUnsecureString());
                var encrKey = CryptoHashing.PBKDF2HMACSHA256(raw,
                                                                Convert.FromBase64String(passwords.EncryptionKeyIvBase64),
                                                                iterations,
                                                                256);
                var macKey = CryptoHashing.PBKDF2HMACSHA256(raw,
                                                            Convert.FromBase64String(passwords.MacKeyIvBase64),
                                                            iterations,
                                                            256);

                var cipher = new AesHmacAuthenticatedCipher(System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
                var passwordsCipher = new AesHmacAuthenticatedCipher.AuthenticatedCiphertext(passwords.EncryptedPasswordsBase64, passwords.TagBase64);
                var encrIv = Convert.FromBase64String(passwords.EncryptionIvBase64);

                var plaintextJson = Encoding.UTF8.GetString(cipher.VerifyMacThenDecrypt(passwordsCipher, encrKey, encrIv, macKey));

                macKey.ZeroOut();
                encrKey.ZeroOut();

                List<StoredPassword> list = JsonConvert.DeserializeObject<List<StoredPassword>>(plaintextJson);

                return list;
            }
            catch(CryptographicException c)
            {
                throw c;
            }
            catch (Exception)
            {
                throw new FormatException("File does not contain a valid format.");
            }

        }

        public async Task<List<StoredPassword>> ImportPasswordsAsync(string filename, SecureString password, IStreamReadToEnd streamReader)
        {
            if (String.IsNullOrEmpty(filename) || password == null)
            {
                throw new ArgumentException("Invalid argument");
            }

            try
            {
                var contents = await streamReader.ReadToEndAsync(filename);

                if (String.IsNullOrEmpty(contents))
                {
                    return new List<StoredPassword>();
                }

                ExportedEncryptedPasswords passwords = JsonConvert.DeserializeObject<ExportedEncryptedPasswords>(contents);

                var iterations = Configuration.ExportImportIterations;
                var raw = Encoding.UTF8.GetBytes(password.ConvertToUnsecureString());
                var encrKey = CryptoHashing.PBKDF2HMACSHA256(raw,
                                                                Convert.FromBase64String(passwords.EncryptionKeyIvBase64),
                                                                iterations,
                                                                256);
                var macKey = CryptoHashing.PBKDF2HMACSHA256(raw,
                                                            Convert.FromBase64String(passwords.MacKeyIvBase64),
                                                            iterations,
                                                            256);

                var cipher = new AesHmacAuthenticatedCipher(System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
                var passwordsCipher = new AesHmacAuthenticatedCipher.AuthenticatedCiphertext(passwords.EncryptedPasswordsBase64, passwords.TagBase64);
                var encrIv = Convert.FromBase64String(passwords.EncryptionIvBase64);

                var plaintextJson = Encoding.UTF8.GetString(cipher.VerifyMacThenDecrypt(passwordsCipher, encrKey, encrIv, macKey));

                macKey.ZeroOut();
                encrKey.ZeroOut();

                List<StoredPassword> list = JsonConvert.DeserializeObject<List<StoredPassword>>(plaintextJson);

                return list;
            }
            catch (CryptographicException c)
            {
                throw c;
            }
            catch (Exception)
            {
                throw new FormatException("File does not contain a valid format.");
            }
        }
    }
}
