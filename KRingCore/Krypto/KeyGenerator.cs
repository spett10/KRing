using KRingCore.Krypto.Model;
using System.Security;
using System.Threading.Tasks;

namespace KRingCore.Krypto
{
    public class KeyGenerator
    {
        private static readonly int HashSaltSizeBytes = 64;

        public Task<KeyGenResult> GetGenerationTask(SecureString password, int iteration)
        {

            var encryptionKeySalt = CryptoHashing.GenerateSalt(HashSaltSizeBytes);
            var macKeySalt = CryptoHashing.GenerateSalt(HashSaltSizeBytes);

            return GetGenerationTask(password, encryptionKeySalt, macKeySalt, iteration);


        }

        public Task<KeyGenResult> GetGenerationTask(SecureString password, byte[] encryptionKeySalt, byte[] macKeySalt, int iteration)
        {
            return Task.Run(() => {
                var encryptionKey = new SymmetricKey(password, encryptionKeySalt, iteration);
                var macKey = new SymmetricKey(password, macKeySalt, iteration);

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
