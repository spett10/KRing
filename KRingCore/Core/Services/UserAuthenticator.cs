using KRingCore.Krypto;
using System;
using System.Text;

namespace KRingCore.Core.Services
{
    public class UserAuthenticator
    {
        private readonly int _loginIterations;
        private readonly int _oldLoginIterations;

        public UserAuthenticator()
        {
            _loginIterations = Configuration.PBKDF2LoginIterations;
            _oldLoginIterations = Configuration.OLD_PBKDF2LoginIterations;
        }

        public UserAuthenticator(int loginIterations, int oldLoginIterations = 1)
        {
            _loginIterations = loginIterations;
            _oldLoginIterations = oldLoginIterations;
        }

        public bool Authenticate(byte[] secret, byte[] salt, byte[] saltedSecret)
        {
            if(secret == null ||salt == null || saltedSecret == null)
            {
                throw new ArgumentException("Null Argument Given");
            }

            var correctSecret = ComputeAndCompareHash(secret, salt, saltedSecret, _loginIterations);

            if (correctSecret) return correctSecret;

            if(!Configuration.TryOldValues)
            {
                return correctSecret;
            }
            else
            {
                return ComputeAndCompareHash(secret, salt, saltedSecret, _oldLoginIterations);
            }
        }
                
        public bool Authenticate(string secret, byte[] salt, byte[] saltedSecret)
        {
            var rawSecret = Encoding.ASCII.GetBytes(secret);
            return Authenticate(rawSecret, salt, saltedSecret);
        }

        public byte[] CreateAuthenticationToken(string secret, byte[] salt)
        {
            return CreateAuthenticationToken(Encoding.ASCII.GetBytes(secret), salt);
        }

        public byte[] CreateAuthenticationToken(byte[] secret, byte[] salt)
        {
            return HashSecret(secret, salt, _loginIterations);
        }

        public bool ComputeAndCompareHash(byte[] secret, byte[] salt, byte[] saltedSecret, int iterations)
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
