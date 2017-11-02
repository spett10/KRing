using System;

namespace KRingCore.Core.Model
{
    public class SecurityData
    {
        public byte[] HashedUsername { get; set; }
        public byte[] UsernameHashSalt { get; set; }
        public byte[] HashedPassword { get; set; }
        public byte[] PasswordHashSalt { get; set; }
        public byte[] EncryptionKeySalt { get; set; }
        public byte[] MacKeySalt { get; set; }

        public SecurityData(byte[] hashedpassword, byte[] hashedUsername, byte[] encrKeySalt, byte[] macKeySalt, byte[] passwordHashSalt, byte[] usernameHashSalt)
        {
            HashedPassword = hashedpassword;
            HashedUsername = hashedUsername;
            EncryptionKeySalt = encrKeySalt;
            MacKeySalt = macKeySalt;
            PasswordHashSalt = passwordHashSalt;
            UsernameHashSalt = usernameHashSalt;
        }

        public SecurityData(string hashedPassword, string hashedUsername, byte[] encrKeySalt, byte[] macKeySalt, byte[] hashSalt, byte[] usernameHashSalt)
        {
            HashedPassword = Convert.FromBase64String(hashedPassword);
            HashedUsername = Convert.FromBase64String(hashedUsername);
            EncryptionKeySalt = encrKeySalt;
            MacKeySalt = macKeySalt;
            PasswordHashSalt = hashSalt;
            UsernameHashSalt = usernameHashSalt;
        }
    }
}
