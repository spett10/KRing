using System;
using System.Security.Cryptography;

namespace KRingCore.Security
{
    /// <summary>
    /// A concrete implementation of the generic base class. Uses AES and HMACSHA256. 
    /// </summary>
    public class AesHmacAuthenticatedCipher : AuthenticatedCipher<AesCryptoServiceProvider, HMACSHA256>
    {
        public AesHmacAuthenticatedCipher(CipherMode cipherMode, PaddingMode paddingMode) : base(cipherMode, paddingMode)
        {
        }

        public struct AuthenticatedCiphertext
        {
            public byte[] ciphertext;
            public byte[] tag;

            public AuthenticatedCiphertext(byte[] ciphertext, byte[] tag)
            {
                this.ciphertext = ciphertext;
                this.tag = tag;
            }

            public AuthenticatedCiphertext(string ciphertext, string tag)
            {
                this.ciphertext = Convert.FromBase64String(ciphertext);
                this.tag = Convert.FromBase64String(tag);
            }

            public string GetCipherAsBase64()
            {
                return Convert.ToBase64String(ciphertext);
            }

            public string GetTagAsBase64()
            {
                return Convert.ToBase64String(tag);
            }
        }

        public AuthenticatedCiphertext EncryptThenHMac(byte[] plaintext, byte[] encryptionIv, byte[] encrKey, byte[] macKey)
        {
            return EncryptThenTag(plaintext, encrKey, encryptionIv, macKey, x => new HMACSHA256(x));
        }

        public byte[] VerifyMacThenDecrypt(AuthenticatedCiphertext ciphertext, byte[] encrKey, byte[] encrIv, byte[] hmacKey)
        {
            return VerifyThenDecrypt(ciphertext, encrKey, encrIv, hmacKey, x => new HMACSHA256(x));
        }
    }
}
