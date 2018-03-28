using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using KRingCore.Persistence.Model;
using System.Security;
using Newtonsoft.Json;
using KRingCore.Security;
using KRingCore.Extensions;
using System.Text;
using KRingCore.Core.Model;

namespace KRingCore.Core.Services
{
    public class EncryptingPasswordExporter : IEncryptingPasswordExporter
    {
        private readonly KeyGenerator _generator;
        private readonly SecureString _password;
        private Task<KeyGenResult> _keyGenTask;

        public EncryptingPasswordExporter(KeyGenerator generator, SecureString password)
        {
            _generator = generator;
            _password = password;
            _keyGenTask = _generator.GetGenerationTask(password);
        }
        
        public string ExportPasswords(List<StoredPassword> passwords)
        {
            var json = JsonConvert.SerializeObject(passwords);

            var iterations = Configuration.ExportImportIterations;
            var raw = Encoding.UTF8.GetBytes(_password.ConvertToUnsecureString());

            var keyGenResult = _keyGenTask.Result;

            var encrKey = keyGenResult.EncryptionKey.Bytes;
            var macKey = keyGenResult.MacKey.Bytes;

            var cipher = new AesHmacAuthenticatedCipher(System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);

            var encryptionIv = CryptoHashing.GenerateSalt(16);
            var ciphertext = cipher.EncryptThenHMac(Encoding.UTF8.GetBytes(json), encryptionIv, encrKey, macKey);

            var payload = new ExportedEncryptedPasswords()
            {
                EncryptedPasswordsBase64 = Convert.ToBase64String(ciphertext.ciphertext),
                EncryptionKeyIvBase64 = Convert.ToBase64String(keyGenResult.SaltForEncryptionKey),
                EncryptionIvBase64 = Convert.ToBase64String(encryptionIv),
                CiphertextTagBase64 = Convert.ToBase64String(ciphertext.tag),
                MacKeyIvBase64 = Convert.ToBase64String(keyGenResult.SaltForMacKey)
            };


            var serializedPayload = payload.ToJsonString();
            var payloadMac = CryptoHashing.HMACSHA256(Encoding.ASCII.GetBytes(serializedPayload), keyGenResult.MacKey);

            var result = new ExportedEncryptedPasswordsWithIntegrity()
            {
                EncryptedPasswordsBase64 = payload.EncryptedPasswordsBase64,
                EncryptionKeyIvBase64 = payload.EncryptionKeyIvBase64,
                EncryptionIvBase64 = payload.EncryptionIvBase64,
                CiphertextTagBase64 = payload.CiphertextTagBase64,
                MacKeyIvBase64 = payload.MacKeyIvBase64,
                PayloadTagBase64 = Convert.ToBase64String(payloadMac)
            };

            keyGenResult.EncryptionKey.Dispose();
            keyGenResult.MacKey.Dispose();

            RestartKeyGen(_password);

            return result.ToJsonString();
        }

        public async Task<string> ExportPasswordsAsync(List<StoredPassword> passwords)
        {
            var json = JsonConvert.SerializeObject(passwords);

            var iterations = Configuration.ExportImportIterations;
            var raw = Encoding.UTF8.GetBytes(_password.ConvertToUnsecureString());

            var keyGenResult = await _keyGenTask;

            var encrKey = keyGenResult.EncryptionKey.Bytes;
            var macKey = keyGenResult.MacKey.Bytes;

            var cipher = new AesHmacAuthenticatedCipher(System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);

            var encryptionIv = CryptoHashing.GenerateSalt(16);
            var ciphertext = cipher.EncryptThenHMac(Encoding.UTF8.GetBytes(json), encryptionIv, encrKey, macKey);

            var payload = new ExportedEncryptedPasswords()
            {
                EncryptedPasswordsBase64 = Convert.ToBase64String(ciphertext.ciphertext),
                EncryptionKeyIvBase64 = Convert.ToBase64String(keyGenResult.SaltForEncryptionKey),
                EncryptionIvBase64 = Convert.ToBase64String(encryptionIv),
                CiphertextTagBase64 = Convert.ToBase64String(ciphertext.tag),
                MacKeyIvBase64 = Convert.ToBase64String(keyGenResult.SaltForMacKey)
            };


            var serializedPayload = payload.ToJsonString();
            var payloadMac = CryptoHashing.HMACSHA256(Encoding.ASCII.GetBytes(serializedPayload), keyGenResult.MacKey);

            var result = new ExportedEncryptedPasswordsWithIntegrity()
            {
                EncryptedPasswordsBase64 = payload.EncryptedPasswordsBase64,
                EncryptionKeyIvBase64 = payload.EncryptionKeyIvBase64,
                EncryptionIvBase64 = payload.EncryptionIvBase64,
                CiphertextTagBase64 = payload.CiphertextTagBase64,
                MacKeyIvBase64 = payload.MacKeyIvBase64,
                PayloadTagBase64 = Convert.ToBase64String(payloadMac)
            };

            keyGenResult.EncryptionKey.Dispose();
            keyGenResult.MacKey.Dispose();

            RestartKeyGen(_password);

            return result.ToJsonString();
        }

        private void RestartKeyGen(SecureString password)
        {
            _keyGenTask = _generator.GetGenerationTask(password);
        }
    }
}
