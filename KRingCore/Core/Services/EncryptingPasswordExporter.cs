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
        //TODO what should this return? we kinda want the form to have the export functionality, but how does it work? do we point to a file or a folder or what.. 
        public string ExportPasswords(List<StoredPassword> passwords, SecureString password)
        {
            var json = JsonConvert.SerializeObject(passwords);

            var iterations = Configuration.ExportImportIterations;
            var raw = Encoding.UTF8.GetBytes(password.ConvertToUnsecureString());
            //TODO: move the below values to config
            var encrKeySalt = CryptoHashing.GenerateSalt(64);
            var encrKey = CryptoHashing.PBKDF2HMACSHA256(raw, encrKeySalt, iterations, 256);
            var macKeySalt = CryptoHashing.GenerateSalt(64);
            var macKey = CryptoHashing.PBKDF2HMACSHA256(raw, macKeySalt, iterations, 256);

            var cipher = new AesHmacAuthenticatedCipher(System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);

            var encryptionIv = CryptoHashing.GenerateSalt(16);
            var ciphertext = cipher.EncryptThenHMac(Encoding.UTF8.GetBytes(json), encryptionIv, encrKey, macKey);

            macKey.ZeroOut();
            encrKey.ZeroOut();

            var result = new ExportedEncryptedPasswords()
            {
                EncryptedPasswordsBase64 = Convert.ToBase64String(ciphertext.ciphertext),
                EncryptionKeyIvBase64 = Convert.ToBase64String(encrKeySalt),
                EncryptionIvBase64 = Convert.ToBase64String(encryptionIv),
                TagBase64 = Convert.ToBase64String(ciphertext.tag),
                MacKeyIvBase64 = Convert.ToBase64String(macKeySalt)
            };

            return JsonConvert.SerializeObject(result);
        }

        public Task<string> ExportPasswordsAsync(List<StoredPassword> passwords, SecureString password)
        {
            throw new NotImplementedException();
        }
    }
}
