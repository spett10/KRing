using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRing.Core
{
    public class Cookie
    {
        public byte[] PasswordSalted { get; set; }
        public byte[] SaltForPassword { get; set; }
        public byte[] KeySalt { get; set; }

        public Cookie(byte[] passwordSalted, byte[] saltForPassword, byte[] keySalt)
        {
            PasswordSalted = passwordSalted;
            SaltForPassword = saltForPassword;
            KeySalt = keySalt;
        }
    }
}
