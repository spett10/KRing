using System;

namespace KRing.Core.Model
{
    public class SecurityData
    {
        public string HashedPassword { get; set; }
        public byte[] KeySalt { get; set; }
        public byte[] HashSalt { get; set; }

        public SecurityData(byte[] hashedpassword, byte[] keySalt, byte[] hashSalt)
        {
            HashedPassword = Convert.ToBase64String(hashedpassword);
            KeySalt = keySalt;
            HashSalt = hashSalt;
        }

        public SecurityData(string hashedPassword, byte[] keySalt, byte[] hashSalt)
        {
            HashedPassword = hashedPassword;
            KeySalt = keySalt;
            HashSalt = hashSalt;
        }
    }
}
