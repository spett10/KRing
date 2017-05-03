using System;

namespace KRing.Core.Model
{
    public class SecurityData
    {
        public string HashedUsername { get; set; }
        public byte[] UsernameHashSalt { get; set; }
        public string HashedPassword { get; set; }
        public byte[] PasswordHashSalt { get; set; }
        public byte[] KeySalt { get; set; }

        public SecurityData(byte[] hashedpassword, byte[] hashedUsername, byte[] keySalt, byte[] passwordHashSalt, byte[] usernameHashSalt)
        {
            HashedPassword = Convert.ToBase64String(hashedpassword);
            HashedUsername = Convert.ToBase64String(hashedUsername);
            KeySalt = keySalt;
            PasswordHashSalt = passwordHashSalt;
            UsernameHashSalt = usernameHashSalt;
        }

        public SecurityData(string hashedPassword, string hashedUsername, byte[] keySalt, byte[] hashSalt, byte[] usernameHashSalt)
        {
            HashedPassword = hashedPassword;
            HashedUsername = hashedUsername;
            KeySalt = keySalt;
            PasswordHashSalt = hashSalt;
            UsernameHashSalt = usernameHashSalt;
        }
    }
}
