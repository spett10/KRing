using System;
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

            keyGenResult.EncryptionKey.Dispose();
            keyGenResult.MacKey.Dispose();
            
            var result = new ExportedEncryptedPasswords()
            {
                EncryptedPasswordsBase64 = Convert.ToBase64String(ciphertext.ciphertext),
                EncryptionKeyIvBase64 = Convert.ToBase64String(keyGenResult.SaltForEncryptionKey),
                EncryptionIvBase64 = Convert.ToBase64String(encryptionIv),
                TagBase64 = Convert.ToBase64String(ciphertext.tag),
                MacKeyIvBase64 = Convert.ToBase64String(keyGenResult.SaltForMacKey)
            };

            RestartKeyGen(_password);

            return JsonConvert.SerializeObject(result);
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

            keyGenResult.EncryptionKey.Dispose();
            keyGenResult.MacKey.Dispose();

            var result = new ExportedEncryptedPasswords()
            {
                EncryptedPasswordsBase64 = Convert.ToBase64String(ciphertext.ciphertext),
                EncryptionKeyIvBase64 = Convert.ToBase64String(keyGenResult.SaltForEncryptionKey),
                EncryptionIvBase64 = Convert.ToBase64String(encryptionIv),
                TagBase64 = Convert.ToBase64String(ciphertext.tag),
                MacKeyIvBase64 = Convert.ToBase64String(keyGenResult.SaltForMacKey)
            };

            RestartKeyGen(_password);

            return JsonConvert.SerializeObject(result);
        }

        private void RestartKeyGen(SecureString password)
        {
            _keyGenTask = _generator.GetGenerationTask(password);
        }
    }
}
