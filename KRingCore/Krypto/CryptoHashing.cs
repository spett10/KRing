using KRingCore.Krypto.Model;
using System;
using System.Security.Cryptography;

namespace KRingCore.Krypto
{
    public static class CryptoHashing
    {
        // Based on IV size for AES, but should be like 64 for password hashing.. 
        public static byte[] GenerateSalt(int size = 16)
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] buffer = new byte[size];
            rng.GetBytes(buffer);

            return buffer;
        }


        /* Since we compare hmac and other sensitive stuff, we want to go through the motions? No reason to leak what index */
        /* they differ at by exiting early. So we go through all indexes regardless, to not leak any information other than */
        /* the length */
        public static bool CompareByteArraysNoTimeLeak(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length) return false;

            bool equal = true;

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    equal = false;
                }
            }

            return equal;
        }

        public static byte[] PBKDF2HMACSHA256(byte[] password, byte[] salt, int iterations, int keyLengthInBits)
        {
            return Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, keyLengthInBits / 8);
        }

        public static byte[] HMACSHA256(byte[] data, byte[] key)
        {
            return KeyedHash(data, key, x => new HMACSHA256(x));
        }

        public static byte[] HMACSHA256(byte[] data, SymmetricKey key)
        {
            return KeyedHash(data, key.Bytes, x => new HMACSHA256(x));
        }

        private static byte[] KeyedHash(byte[] data, byte[] key, Func<byte[], KeyedHashAlgorithm> keyedHashCreator)
        {
            using (KeyedHashAlgorithm keyedHash = keyedHashCreator(key))
            {
                return keyedHash.ComputeHash(data);
            }

        }

        public class IntegrityException : Exception
        {

        }
    }
}
