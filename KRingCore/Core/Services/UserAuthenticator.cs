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

        private static bool ComputeAndCompareHash(byte[] secret, byte[] salt, byte[] saltedSecret, int iterations)
        {
            var computedHash = CryptoHashing.PBKDF2HMACSHA256(secret, salt, iterations, 256);
            return CryptoHashing.CompareByteArraysNoTimeLeak(computedHash, saltedSecret);
        }
    }
}
