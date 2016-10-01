using System;

namespace KRing.Core.Model
{
    public class Cookie
    {
        public string HashedPassword { get; set; }
        public byte[] KeySalt { get; set; }

        public Cookie(byte[] hashedpassword, byte[] keySalt)
        {
            HashedPassword = Convert.ToBase64String(hashedpassword);
            KeySalt = keySalt;
        }

        public Cookie(string hashedPassword, byte[] keySalt)
        {
            HashedPassword = hashedPassword;
            KeySalt = keySalt;
        }
    }
}
