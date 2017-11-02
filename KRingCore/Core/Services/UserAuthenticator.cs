using KRingCore.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRingCore.Core.Services
{
    public static class UserAuthenticator
    {
        public static bool Authenticate(byte[] secret, byte[] salt, byte[] saltedSecret)
        {
            if(secret == null ||salt == null || saltedSecret == null)
            {
                throw new ArgumentException("Null Argument Given");
            }

            var iterations = Configuration.PBKDF2LoginIterations;
            var correctSecret = ComputeAndCompareHash(secret, salt, saltedSecret, iterations);

            if (correctSecret) return correctSecret;

            if(!Configuration.TryOldValues)
            {
                return correctSecret;
            }
            else
            {
                var oldIterations = Configuration.OLD_PBKDF2LoginIterations;
                return ComputeAndCompareHash(secret, salt, saltedSecret, oldIterations);
            }
        }
                
        public static bool Authenticate(string secret, byte[] salt, byte[] saltedSecret)
        {
            var rawSecret = Encoding.ASCII.GetBytes(secret);
            return Authenticate(rawSecret, salt, saltedSecret);
        }

        public static byte[] CreateAuthenticationToken(string secret, byte[] salt)
        {
            return CreateAuthenticationToken(Encoding.ASCII.GetBytes(secret), salt);
        }

        public static byte[] CreateAuthenticationToken(byte[] secret, byte[] salt)
        {
            var iterations = Configuration.PBKDF2LoginIterations;
            return HashSecret(secret, salt, iterations);
        }

        private static bool ComputeAndCompareHash(byte[] secret, byte[] salt, byte[] saltedSecret, int iterations)
        {
            var computedHash = HashSecret(secret, salt, iterations);
            return CryptoHashing.CompareByteArraysNoTimeLeak(computedHash, saltedSecret);
        }

        private static byte[] HashSecret(byte[] secret, byte[] salt, int iterations)
        {
            return CryptoHashing.PBKDF2HMACSHA256(secret, salt, iterations, 256);
        }
    }
}
