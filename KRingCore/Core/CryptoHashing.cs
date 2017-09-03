using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using KRingCore.Extensions;
using System.IO;
using KRingCore.Persistence.Model;

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

    public class Aes256AuthenticatedCipher
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
            if (CryptoHashing.CompareByteArrays(encrKey, macKey))
            {
                throw new ArgumentException("Using same key for encryption and mac is insecure");
            }

            var cipher = new AuthenticatedCiphertext();

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = encrKey;
                aes.IV = encryptionIv;

                using (MemoryStream ms = new MemoryStream())
                using (var encryptor = aes.CreateEncryptor())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plaintext, 0, plaintext.Length);
                    cs.FlushFinalBlock();
                    cipher.ciphertext = ms.ToArray();
                }
            }

            using (var hmac = new HMACSHA256(macKey))
            {
                cipher.tag = hmac.ComputeHash(cipher.ciphertext);
            }

            return cipher;
        }

        public static byte[] VerifyMacThenCBCDecrypt(AuthenticatedCiphertext ciphertext, byte[] encrKey, byte[] encrIv, byte[] hmacKey)
        {
            if (CryptoHashing.CompareByteArrays(encrKey, hmacKey))
            {
                throw new ArgumentException("Using same key for encryption and mac is insecure");
            }

            using (var hmac = new HMACSHA256(hmacKey))
            {
                var computedHash = hmac.ComputeHash(ciphertext.ciphertext);
                if(!CryptoHashing.CompareByteArrays(computedHash, ciphertext.tag))
                {
                    throw new CryptographicException("Invalid HMAC");
                }
            }

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = encrKey;
                aes.IV = encrIv;

                using (MemoryStream ms = new MemoryStream())
                using (var encryptor = aes.CreateDecryptor())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(ciphertext.ciphertext, 0, ciphertext.ciphertext.Length);
                    cs.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }
    }
}
