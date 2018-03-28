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
        private readonly KeyGenerator _generator;
        private readonly SecureString _password;

        public DecryptingPasswordImporter(KeyGenerator generator, SecureString password)
        {
            _generator = generator;
            _password = password;
        }

        public List<StoredPassword> ImportPasswords(string filename, IStreamReadToEnd streamReader)
        {
            ValidateInput(filename);

            try
            {
                var contents = streamReader.ReadToEnd(filename);

                if (String.IsNullOrEmpty(contents))
                {
                    return new List<StoredPassword>();
                }

                ExportedEncryptedPasswordsWithIntegrity passwords = JsonConvert.DeserializeObject<ExportedEncryptedPasswordsWithIntegrity>(contents);

                var iterations = Configuration.ExportImportIterations;
                var raw = Encoding.UTF8.GetBytes(_password.ConvertToUnsecureString());

                var keyGenResult = _generator.GetGenerationTask(_password, Convert.FromBase64String(passwords.EncryptionKeyIvBase64), Convert.FromBase64String(passwords.MacKeyIvBase64)).Result;

                var encrKey = keyGenResult.EncryptionKey.Bytes;
                var macKey = keyGenResult.MacKey.Bytes;
                
                ExportedEncryptedPasswords payload = passwords.GetPayload();
                var storedMac = Convert.FromBase64String(passwords.PayloadTagBase64);
                var serializedPayload = payload.ToJsonString();
                var computedMac = CryptoHashing.HMACSHA256(Encoding.ASCII.GetBytes(serializedPayload), keyGenResult.MacKey);

                if(!CryptoHashing.CompareByteArraysNoTimeLeak(storedMac, computedMac))
                {
                    throw new CryptoHashing.IntegrityException();
                }

                var cipher = new AesHmacAuthenticatedCipher(System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
                var passwordsCipher = new AesHmacAuthenticatedCipher.AuthenticatedCiphertext(passwords.EncryptedPasswordsBase64, passwords.CiphertextTagBase64);
                var encrIv = Convert.FromBase64String(passwords.EncryptionIvBase64);

                var plaintextJson = Encoding.UTF8.GetString(cipher.VerifyMacThenDecrypt(passwordsCipher, encrKey, encrIv, macKey));

                keyGenResult.EncryptionKey.Dispose();
                keyGenResult.MacKey.Dispose();

                List<StoredPassword> list = JsonConvert.DeserializeObject<List<StoredPassword>>(plaintextJson);

                return list;
            }
            catch(CryptographicException c)
            {
                throw c;
            }
            catch(CryptoHashing.IntegrityException c)
            {
                throw c;
            }
            catch (Exception)
            {
                throw new FormatException("File does not contain a valid format.");
            }

        }

        public async Task<List<StoredPassword>> ImportPasswordsAsync(string filename, IStreamReadToEnd streamReader)
        {
            ValidateInput(filename);

            try
            {
                var contents = await streamReader.ReadToEndAsync(filename);

                if (String.IsNullOrEmpty(contents))
                {
                    return new List<StoredPassword>();
                }

                ExportedEncryptedPasswordsWithIntegrity passwords = JsonConvert.DeserializeObject<ExportedEncryptedPasswordsWithIntegrity>(contents);

                var iterations = Configuration.ExportImportIterations;
                var raw = Encoding.UTF8.GetBytes(_password.ConvertToUnsecureString());
                var keyGenResult = await _generator.GetGenerationTask(_password, Convert.FromBase64String(passwords.EncryptionKeyIvBase64), Convert.FromBase64String(passwords.MacKeyIvBase64));

                var encrKey = keyGenResult.EncryptionKey.Bytes;
                var macKey = keyGenResult.MacKey.Bytes;
                
                ExportedEncryptedPasswords payload = passwords.GetPayload();
                var storedMac = Convert.FromBase64String(passwords.PayloadTagBase64);
                var computedMac = CryptoHashing.HMACSHA256(Encoding.ASCII.GetBytes(payload.ToJsonString()), macKey);

                if (!CryptoHashing.CompareByteArraysNoTimeLeak(storedMac, computedMac))
                {
                    throw new CryptoHashing.IntegrityException();
                }

                var cipher = new AesHmacAuthenticatedCipher(System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
                var passwordsCipher = new AesHmacAuthenticatedCipher.AuthenticatedCiphertext(passwords.EncryptedPasswordsBase64, passwords.CiphertextTagBase64);
                var encrIv = Convert.FromBase64String(passwords.EncryptionIvBase64);

                var plaintextJson = Encoding.UTF8.GetString(cipher.VerifyMacThenDecrypt(passwordsCipher, encrKey, encrIv, macKey));

                keyGenResult.EncryptionKey.Dispose();
                keyGenResult.MacKey.Dispose();

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

        private void ValidateInput(string filename)
        {
            if (String.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("Invalid argument");
            }
        }
    }
}
