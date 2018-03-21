using KRingCore.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace KRingCore.Security
{
    public class KeyGenerator
    {
        private static readonly int HashSaltSizeBytes = 64;

        public Task<KeyGenResult> GetGenerationTask(SecureString password)
        {

            var encryptionKeySalt = CryptoHashing.GenerateSalt(HashSaltSizeBytes);
            var macKeySalt = CryptoHashing.GenerateSalt(HashSaltSizeBytes);

            return GetGenerationTask(password, encryptionKeySalt, macKeySalt);


        }

        public Task<KeyGenResult> GetGenerationTask(SecureString password, byte[] encryptionKeySalt, byte[] macKeySalt)
        {
            return Task.Run(() => {
                var encryptionKey = new SymmetricKey(password, encryptionKeySalt);
                var macKey = new SymmetricKey(password, macKeySalt);

                return new KeyGenResult()
                {
                    EncryptionKey = encryptionKey,
                    MacKey = macKey,
                    SaltForEncryptionKey = encryptionKeySalt,
                    SaltForMacKey = macKeySalt
                };
            });
        }
    }

}
