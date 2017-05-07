using System;

namespace KRing.Core.Model
{
    public class SecurityData
    {
        public string HashedUsername { get; set; }
        public byte[] UsernameHashSalt { get; set; }
        public string HashedPassword { get; set; }
        public byte[] PasswordHashSalt { get; set; }
        public byte[] EncryptionKeySalt { get; set; }
        public byte[] MacKeySalt { get; set; }

        public SecurityData(byte[] hashedpassword, byte[] hashedUsername, byte[] encrKeySalt, byte[] macKeySalt, byte[] passwordHashSalt, byte[] usernameHashSalt)
        {
            HashedPassword = Convert.ToBase64String(hashedpassword);
            HashedUsername = Convert.ToBase64String(hashedUsername);
            EncryptionKeySalt = encrKeySalt;
            MacKeySalt = macKeySalt;
            PasswordHashSalt = passwordHashSalt;
            UsernameHashSalt = usernameHashSalt;
        }

        public SecurityData(string hashedPassword, string hashedUsername, byte[] encrKeySalt, byte[] macKeySalt, byte[] hashSalt, byte[] usernameHashSalt)
        {
            HashedPassword = hashedPassword;
            HashedUsername = hashedUsername;
            EncryptionKeySalt = encrKeySalt;
            MacKeySalt = macKeySalt;
            PasswordHashSalt = hashSalt;
            UsernameHashSalt = usernameHashSalt;
        }
    }
}
