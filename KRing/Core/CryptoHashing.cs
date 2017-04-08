using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using KRing.Extensions;
using Scrypt;
using Security.Cryptography;
using System.IO;
using KRing.Persistence.Model;

namespace KRing.Core
{
    public static class CryptoHashing
    {
        public static readonly int SaltByteSize = 16;
        private static readonly int iterations = 10000; //could we go further? like.. 20k? 30k? We only have to load them once. 

        public static string ScryptHashPassword(SecureString password)
        {
            ScryptEncoder encoder = new ScryptEncoder();
            return encoder.Encode(password.ConvertToUnsecureString());
        }

        public static bool ScryptCheckPassword(SecureString password, string hashedPassword)
        {
            ScryptEncoder encoder = new ScryptEncoder();
            return encoder.Compare(password.ConvertToUnsecureString(), hashedPassword);
        }

        public static string ScryptHashPassword(SecureString password, byte[] salt)
        {
            ScryptEncoder encoder = new ScryptEncoder();
            return encoder.Encode(Convert.ToBase64String(salt) + password.ConvertToUnsecureString());
        }

        public static bool ScryptCheckPassword(SecureString password, byte[] salt, string hashedPassword)
        {
            ScryptEncoder encoder = new ScryptEncoder();
            return encoder.Compare(Convert.ToBase64String(salt) + password.ConvertToUnsecureString(), hashedPassword);
        }

        public static byte[] GenerateSaltedHash(string plaintext, byte[] salt)
        {
            Rfc2898DeriveBytes algorithm = new Rfc2898DeriveBytes(plaintext, salt, iterations);
            try
            {
                return algorithm.GetBytes(plaintext.Length + salt.Length);
            }
            finally
            {
                algorithm.Dispose();
            }

        }

        public static byte[] GenerateSaltedHash(SecureString plaintext, byte[] salt)
        {
            Rfc2898DeriveBytes algorithm = new Rfc2898DeriveBytes(plaintext.ConvertToUnsecureString(), salt, iterations);
            try
            {
                return algorithm.GetBytes(plaintext.Length + salt.Length);
            }
            finally
            {
                algorithm.Dispose();
            }
        }

        public static byte[] DeriveKeyFromPasswordAndSalt(SecureString plaintext, byte[] salt, int keyLength)
        {
            Rfc2898DeriveBytes algorithm = new Rfc2898DeriveBytes(plaintext.ConvertToUnsecureString(), salt, iterations);
            try
            {
                return algorithm.GetBytes(keyLength);
            }
            finally
            {
                algorithm.Dispose();
            }
        }

        public static byte[] GenerateSalt(int size)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[size];
            try
            {
                rng.GetBytes(buffer);

                return buffer;
            }
            finally
            {
                rng.Dispose();
            }
        }

        public static byte[] GenerateSalt()
        {
            return GenerateSalt(SaltByteSize);
        }

        public static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            return array1.Length == array2.Length && !array1.Where((t, i) => t != array2[i]).Any();
        }


    }

    public class Aes256AuthenticatedCipher
    {
        private string domainBase64;
        private string domainTagBase64;

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

        public static AuthenticatedCiphertext Encrypt(byte[] plaintext, byte[] key, byte[] iv)
        {
            if (iv.Length != 12) throw new ArgumentException(nameof(iv) + " must be 12 bytes long");
            if (key.Length != 32) throw new ArgumentException(nameof(key) + " must be 32 bytes long");

            using (AuthenticatedAesCng aes = new AuthenticatedAesCng())
            {
                aes.CngMode = CngChainingMode.Gcm;

                aes.Key = key;
                aes.IV = iv;

                using (MemoryStream ms = new MemoryStream())
                using(IAuthenticatedCryptoTransform encrypt = aes.CreateAuthenticatedEncryptor())
                using(CryptoStream cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    cs.Write(plaintext, 0, plaintext.Length);
                    cs.FlushFinalBlock();
                    var tag = encrypt.GetTag();
                    var cipher = ms.ToArray();

                    return new AuthenticatedCiphertext(cipher, tag);
                }
            }
        }

        public static byte[] Decrypt(AuthenticatedCiphertext ciphertext, byte[] key, byte[] iv)
        {
            if (iv.Length != 12) throw new ArgumentException(nameof(iv) + " must be 12 bytes long");
            if (key.Length != 32) throw new ArgumentException(nameof(key) + " must be 32 bytes long");

            using (AuthenticatedAesCng aes = new AuthenticatedAesCng())
            {
                aes.CngMode = CngChainingMode.Gcm;

                aes.Key = key;
                aes.IV = iv;
                aes.Tag = ciphertext.tag;

                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(ciphertext.ciphertext, 0, ciphertext.ciphertext.Length);

                    //If the authentication tag does not match, we'll fail here with a 
                    //CryptographicException, and the ciphertext will not be decrypted
                    cs.FlushFinalBlock();

                    return ms.ToArray();
                }
            }
        }
    }
}
