using System;
using System.Security;
using System.Security.Cryptography;
using KRingCore.Extensions;
using Org.BouncyCastle.Crypto.Generators;
using System.IO;
using static KRingCore.Security.AesHmacAuthenticatedCipher;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using KRingCore.Core;
using System.Text;

namespace KRingCore.Security
{
    public static class CryptoHashing
    {
        // Based on IV size for AES, but should be like 64 for password hashing.. 
        public static readonly int DefaultSaltByteSize = 16;

        public static byte[] DeriveKeyFromPasswordAndSalt(SecureString password, byte[] salt, int keyLength)
        {
            var raw = Encoding.ASCII.GetBytes(password.ConvertToUnsecureString());
            var iterations = Configuration.PBKDF2DeriveIterations;
            // This algorithm takes keysize at bits, not bytes, so mult by 8.
            return PBKDF2HMACSHA256(raw, salt, iterations, keyLength * 8);
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
            return GenerateSalt(DefaultSaltByteSize);
        }

        /* Since we compare hmac and other sensitive stuff, we want to go through the motions? No reason to leak what index */
        /* they differ at by exiting early. So we go through all indexes regardless, to not leak any information other than */ 
        /* the length */
        public static bool CompareByteArraysNoTimeLeak(byte[] array1, byte[] array2)
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

        public static byte[] PBKDF2HMACSHA256(byte[] password, byte[] salt, int rounds, int keyLengthInBits)
        {
            Pkcs5S2ParametersGenerator gen = new Pkcs5S2ParametersGenerator(new Sha256Digest());
            gen.Init(password, salt, rounds);            
            return ((KeyParameter)gen.GenerateDerivedMacParameters(keyLengthInBits)).GetKey();
        }

        public static byte[] HMACSHA256(byte[] data, byte[] key)
        {
            return KeyedHash(data, key, x => new HMACSHA256(x));
        }

        private static byte[] KeyedHash(byte[] data, byte[] key, Func<byte[], KeyedHashAlgorithm> keyedHashCreator)
        {
            using (KeyedHashAlgorithm keyedHash = keyedHashCreator(key))
            {
                return keyedHash.ComputeHash(data);
            }

        }
    }
}
