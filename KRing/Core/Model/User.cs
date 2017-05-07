using System.Security;
using KRing.Extensions;

namespace KRing.Core.Model
{
    public class User
    {
        public string UserName { get; set; }
        public string PlaintextPassword
        {
            get
            {
                var raw = Password.ConvertToUnsecureString();
                PlaintextPassword = raw;
                return raw;
            }

            set
            {
                Password = new SecureString();
                Password.PopulateWithString(value);
            }
        }
        private SecureString Password { get; set; }
        public SecurityData Cookie { get; private set; }

        private static readonly int HashSaltSize = 64;

        public User(string name, SecureString password, SecurityData cookie)
        {
            UserName = name;
            Password = password;
            Cookie = cookie;
        }

        public User(string name, SecurityData cookie)
        {
            UserName = name;
            Password = new SecureString();
            Cookie = cookie;
        }

        public static User NewUserWithFreshSalt(string newUserName, SecureString password)
        {
            var rawPass = password.ConvertToUnsecureString();
            password = new SecureString();
            password.PopulateWithString(rawPass);

            var saltForUser = CryptoHashing.GenerateSalt(HashSaltSize);
            var saltedUsername = CryptoHashing.GenerateSaltedHash(newUserName, saltForUser);

            var saltForHash = CryptoHashing.GenerateSalt(HashSaltSize);
            var saltedPassword = CryptoHashing.GenerateSaltedHash(rawPass,saltForHash);
            var saltForKey = CryptoHashing.GenerateSalt();

            var cookie = new SecurityData(saltedPassword, saltedUsername, saltForKey, saltForHash, saltForUser);
            return new User(newUserName, password, cookie);
        }

        public static User DummyUser()
        {
            return new User("Dummy", new SecureString(),
                                    new SecurityData(CryptoHashing.GenerateSalt(),
                                                CryptoHashing.GenerateSalt(),
                                                CryptoHashing.GenerateSalt(),
                                                CryptoHashing.GenerateSalt(HashSaltSize),
                                                CryptoHashing.GenerateSalt()));
        }
        
        public override string ToString()
        {
            return UserName;
        }

    }
}
