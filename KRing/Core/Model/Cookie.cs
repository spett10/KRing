using System;

namespace KRing.Core.Model
{
    public class Cookie
    {
        public string PasswordSalted { get; set; }
        public byte[] KeySalt { get; set; }

        public Cookie(byte[] passwordSalted, byte[] keySalt)
        {
            PasswordSalted = Convert.ToBase64String(passwordSalted);
            KeySalt = keySalt;
        }

        public Cookie(string passwordSalted, byte[] keySalt)
        {
            PasswordSalted = passwordSalted;
            KeySalt = keySalt;
        }
    }
}
