namespace KRing.Core.Model
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
