using System.Security;
using KRingCore.Core.Services;
using KRingCore.Krypto;
using KRingCore.Krypto.Extensions;

namespace KRingCore.Core.Model
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
        public SecureString Password { get; private set; }
        public SecurityData SecurityData { get; private set; }

        private static readonly int HashSaltSize = 64;

        public User(string name, string password, SecurityData securityData)
        {
            UserName = name;
            SecurityData = securityData;
            Password = new SecureString();
            foreach(var c in password)
            {
                Password.AppendChar(c);
            }
        }

        public User(string name, SecureString password, SecurityData securityData)
        {
            UserName = name;
            Password = password;
            SecurityData = securityData;
        }

        public User(string name, SecurityData securityData)
        {
            UserName = name;
            Password = new SecureString();
            SecurityData = securityData;
        }

        public void GenerateNewSalt(UserAuthenticator userAuthenticator)
        {
            var securityData = new SecurityData()
            {
                PasswordHashSalt = CryptoHashing.GenerateSalt(HashSaltSize),
                UsernameHashSalt = CryptoHashing.GenerateSalt(HashSaltSize),
            };

            securityData.HashedPassword = userAuthenticator.
                                          CreateAuthenticationToken(this.Password.ConvertToUnsecureString(), 
                                                                    securityData.PasswordHashSalt);

            securityData.HashedUsername = userAuthenticator.
                                          CreateAuthenticationToken(this.UserName,
                                                                    securityData.UsernameHashSalt);

            this.SecurityData = securityData;
        }

        public static User NewUserWithFreshSalt(UserAuthenticator userAuthenticator, string newUserName, SecureString password)
        {
            return NewUserWithFreshSalt(userAuthenticator, newUserName, password.ConvertToUnsecureString());
        }

        public static User NewUserWithFreshSalt(UserAuthenticator userAuthenticator, string newUserName, string password)
        {
            var saltForUser = CryptoHashing.GenerateSalt(HashSaltSize);
            var saltedUsername = userAuthenticator.CreateAuthenticationToken(newUserName, saltForUser);

            var saltForPassword = CryptoHashing.GenerateSalt(HashSaltSize);
            var saltedPassword = userAuthenticator.CreateAuthenticationToken(password, saltForPassword);

            var cookie = new SecurityData(saltedPassword, saltedUsername, saltForPassword, saltForUser);
            return new User(newUserName, password, cookie);
        }

        public static User DummyUser()
        {
            return new User("Dummy", new SecureString(),
                                    new SecurityData(CryptoHashing.GenerateSalt(),
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
