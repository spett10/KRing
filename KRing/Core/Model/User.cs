using System.Security;
using KRing.Extensions;

namespace KRing.Core.Model
{
    public class User
    {
        public string UserName { get; private set; }
        public SecureString Password { get; set; }
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
            var saltForHash = CryptoHashing.GenerateSalt(HashSaltSize);
            var saltedPassword = CryptoHashing.ScryptHashPassword(password,saltForHash);
            var saltForKey = CryptoHashing.GenerateSalt();

            var cookie = new SecurityData(saltedPassword, saltForKey, saltForHash);
            return new User(newUserName, password, cookie);
        }

        public static User DummyUser()
        {
            return new User("Dummy", new SecureString(),
                                    new SecurityData(CryptoHashing.GenerateSalt(),
                                                CryptoHashing.GenerateSalt(),
                                                CryptoHashing.GenerateSalt(HashSaltSize)));
        }
        
        public override string ToString()
        {
            return UserName;
        }

    }
}
