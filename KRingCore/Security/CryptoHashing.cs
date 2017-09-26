using System;
using System.Security;
using System.Security.Cryptography;
using KRingCore.Extensions;
using Org.BouncyCastle.Crypto.Generators;
using System.IO;
using static KRingCore.Security.AesHmacAuthenticatedCipher;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;

namespace KRingCore.Security
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
                return CompareByteArraysNoTimeLeak(hashed, Convert.FromBase64String(hashedPassword));
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

        public static byte[] PBKDF2HMACSHA256(byte[] password, byte[] salt, int rounds, int keylength)
        {
            Pkcs5S2ParametersGenerator gen = new Pkcs5S2ParametersGenerator(new Sha256Digest());
            gen.Init(password, salt, rounds);            
            return ((KeyParameter)gen.GenerateDerivedMacParameters(keylength)).GetKey();
        }
    }
}
