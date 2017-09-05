using System;
using System.Security;
using System.Security.Cryptography;
using KRingCore.Extensions;
using System.IO;
using static KRingCore.Core.AesHmacAuthenticatedCipher;

namespace KRingCore.Core
{
    public static class CryptoHashing
    {
        public static readonly int SaltByteSize = 16;
        private static readonly int iterations = 10000; //could we go further? like.. 20k? 30k? We only have to load them once. 

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

        /* Assumes base64 encoding  of input strings */
        public static bool CompareSaltedHash(string password, byte[] salt, string hashedPassword)
        {
            Rfc2898DeriveBytes algorithm = new Rfc2898DeriveBytes(password, salt, iterations);
            try
            {
                var hashed = algorithm.GetBytes(password.Length + salt.Length);
                return CompareByteArrays(hashed, Convert.FromBase64String(hashedPassword));
            }
            finally
            {
                algorithm.Dispose();
            }
        }

        public static byte[] DeriveKeyFromPasswordAndSalt(string plaintext, byte[] salt, int keyLength)
        {
            Rfc2898DeriveBytes algorithm = new Rfc2898DeriveBytes(plaintext, salt, iterations);
            try
            {
                return algorithm.GetBytes(keyLength);
            }
            finally
            {
                algorithm.Dispose();
            }
        }

        public static byte[] DeriveKeyFromPasswordAndSalt(SecureString password, byte[] salt, int keyLength)
        {
            var raw = password.ConvertToUnsecureString();
            Rfc2898DeriveBytes algorithm = new Rfc2898DeriveBytes(raw, salt, iterations);
            try
            {
                return algorithm.GetBytes(keyLength);
            }
            finally
            {
                //TODO: should this be in the caller instead, and we take a string, or pref. a char array?
                password = new SecureString();
                password.PopulateWithString(raw);
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

        /* Since we compare hmac and other sensitive stuff, we want to go through the motions? No reason to leak what index */
        /* they differ at by exiting early. So we go through all indexes regardless, to not leak any information other than */ 
        /* the length */
        public static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length) return false;

            bool equal = true;

            for(int i = 0; i < array1.Length; i++)
            {
                if(array1[i] != array2[i])
                {
                    equal = false;
                }
            }

            return equal;
        }

        public static void ZeroOutArray(ref byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 0;
            }
        }
    }

    /// <summary>
    /// Base class for an authenticated cipher. Needs a symmetric encryption algorithm and a keyed hash algorithm. 
    /// Uses CBC and PKCS7 mode by default. The idea is that we can extend it in the future, say, going to another HMAC
    /// if the used one is not good enough, or away from some symmetric algorithm. 
    /// 
    /// TODO: How do we extract the ciphermode and the padding? We cant use enum in constrants.. More delegates? There are just so many.
    /// Maybe an interface in, that has 3 delegates, one for ciphermode, one for padding, one for hmaccreation, then we only have one argument. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    public abstract class AuthenticatedCipherCbcPkcs7<T, V> where T: SymmetricAlgorithm, new() where V : KeyedHashAlgorithm
    {
        internal static AuthenticatedCiphertext EncryptThenTag(byte[] plaintext, byte[] encrKey, byte[] iv, byte[] hmacKey, Func<byte[], V> keyedHashCreator)
        {
            if (CryptoHashing.CompareByteArrays(encrKey, hmacKey))
            {
                throw new ArgumentException("Using same key for encryption and mac is insecure");
            }

            var cipher = new AuthenticatedCiphertext();

            using (T symmetric = new T())
            {
                symmetric.Mode = CipherMode.CBC;
                symmetric.Padding = PaddingMode.PKCS7;
                symmetric.Key = encrKey;
                symmetric.IV = iv;

                using (MemoryStream ms = new MemoryStream())
                using (var encryptor = symmetric.CreateEncryptor())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plaintext, 0, plaintext.Length);
                    cs.FlushFinalBlock();
                    cipher.ciphertext = ms.ToArray();
                }
            }

            using (V keyedHash = keyedHashCreator(hmacKey))
            {
                cipher.tag = keyedHash.ComputeHash(cipher.ciphertext);
            }

            return cipher;
        }

        internal static byte[] VerifyThenDecrypt(AuthenticatedCiphertext ciphertext, byte[] encrKey, byte[] iv, byte[] hmacKey, Func<byte[], V> keyedHashCreator)
        {
            if (CryptoHashing.CompareByteArrays(encrKey, hmacKey))
            {
                throw new ArgumentException("Using same key for encryption and mac is insecure");
            }

            using (var hmac = keyedHashCreator(hmacKey))
            {
                var computedHash = hmac.ComputeHash(ciphertext.ciphertext);
                if (!CryptoHashing.CompareByteArrays(computedHash, ciphertext.tag))
                {
                    throw new CryptographicException("Invalid HMAC");
                }
            }

            using (T symmetric = new T())
            {
                symmetric.Mode = CipherMode.CBC;
                symmetric.Padding = PaddingMode.PKCS7;
                symmetric.Key = encrKey;
                symmetric.IV = iv;

                using (MemoryStream ms = new MemoryStream())
                using (var encryptor = symmetric.CreateDecryptor())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(ciphertext.ciphertext, 0, ciphertext.ciphertext.Length);
                    cs.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }
    }

    /// <summary>
    /// A concrete implementation of the generic base class. Uses AES and HMACSHA256. 
    /// </summary>
    public class AesHmacAuthenticatedCipher : AuthenticatedCipherCbcPkcs7<AesCryptoServiceProvider, HMACSHA256>
    {
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

        public static AuthenticatedCiphertext CBCEncryptThenHMac(byte[] plaintext, byte[] encryptionIv, byte[] encrKey, byte[] macKey)
        {
            return EncryptThenTag(plaintext, encrKey, encryptionIv, macKey, x => new HMACSHA256(x));
        }

        public static byte[] VerifyMacThenCBCDecrypt(AuthenticatedCiphertext ciphertext, byte[] encrKey, byte[] encrIv, byte[] hmacKey)
        {
            return VerifyThenDecrypt(ciphertext, encrKey, encrIv, hmacKey, x => new HMACSHA256(x));
        }
    }
}
