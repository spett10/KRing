using Krypto.Model;

namespace Krypto.KeyGen
{
    public class KeyGenResult
    {
        public SymmetricKey EncryptionKey { get; set; }
        public SymmetricKey MacKey { get; set; }
        public byte[] SaltForEncryptionKey { get; set; }
        public byte[] SaltForMacKey { get; set; }
    }
}
