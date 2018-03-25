using System;

namespace KRingCore.Core.Model
{
    public class SecurityData
    {
        public byte[] HashedUsername { get; set; }
        public byte[] UsernameHashSalt { get; set; }
        public byte[] HashedPassword { get; set; }
        public byte[] PasswordHashSalt { get; set; }

        public SecurityData()
        {

        }

        public SecurityData(byte[] hashedpassword, byte[] hashedUsername, byte[] passwordHashSalt, byte[] usernameHashSalt)
        {
            HashedPassword = hashedpassword;
            HashedUsername = hashedUsername;
            PasswordHashSalt = passwordHashSalt;
            UsernameHashSalt = usernameHashSalt;
        }

        public SecurityData(string hashedPassword, string hashedUsername, byte[] hashSalt, byte[] usernameHashSalt)
        {
            HashedPassword = Convert.FromBase64String(hashedPassword);
            HashedUsername = Convert.FromBase64String(hashedUsername);
            PasswordHashSalt = hashSalt;
            UsernameHashSalt = usernameHashSalt;
        }
    }
}
