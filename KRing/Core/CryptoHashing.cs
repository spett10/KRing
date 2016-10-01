using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using KRing.Extensions;
using Scrypt;

namespace KRing.Core
{
    public static class CryptoHashing
    {
        public static readonly int SaltByteSize = 16;
        private static readonly int iterations = 10000;

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
}
