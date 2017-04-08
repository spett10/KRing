using System.Security;
using KRing.Extensions;

namespace KRing.Core.Model
{
    public class User
    {
        public string UserName { get; private set; }
        public SecureString Password { get; set; }
        public Cookie Cookie { get; private set; }

        public User(string name, SecureString password, Cookie cookie)
        {
            UserName = name;
            Password = password;
            Cookie = cookie;
        }

        public User(string name, Cookie cookie)
        {
            UserName = name;
            Password = new SecureString();
            Cookie = cookie;
        }

        public static User NewUserWithFreshSalt(string newUserName, SecureString password)
        {
            var saltForHash = CryptoHashing.GenerateSalt(64);
            var saltedPassword = CryptoHashing.ScryptHashPassword(password,saltForHash);
            var saltForKey = CryptoHashing.GenerateSalt();

            var cookie = new Cookie(saltedPassword, saltForKey, saltForHash);
            return new User(newUserName, password, cookie);
        }

        public static User DummyUser()
        {
            return new User("Dummy", new SecureString(),
                                    new Cookie(CryptoHashing.GenerateSalt(),
                                                CryptoHashing.GenerateSalt(),
                                                CryptoHashing.GenerateSalt()));
        }
        
        public override string ToString()
        {
            return UserName;
        }

    }
}
