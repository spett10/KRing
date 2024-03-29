﻿using System.Security;
using Krypto.Extensions;
using Krypto;
using KRingCore.Core.Services;

namespace KRingCore.Core.Model
{
    public class User
    {
        private const int DefaultSaltByteSize = 32;

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

        public User(string name, string password, SecurityData cookie)
        {
            UserName = name;
            SecurityData = cookie;
            Password = new SecureString();
            foreach(var c in password)
            {
                Password.AppendChar(c);
            }
        }

        public User(string name, SecureString password, SecurityData cookie)
        {
            UserName = name;
            Password = password;
            SecurityData = cookie;
        }

        public User(string name, SecurityData cookie)
        {
            UserName = name;
            Password = new SecureString();
            SecurityData = cookie;
        }

        public void GenerateNewSalt()
        {
            var securityData = new SecurityData()
            {
                PasswordHashSalt = CryptoHashing.GenerateSalt(HashSaltSize),
                UsernameHashSalt = CryptoHashing.GenerateSalt(HashSaltSize),
            };

            securityData.HashedPassword = UserAuthenticator.
                                          CreateAuthenticationToken(this.Password.ConvertToUnsecureString(), 
                                                                    securityData.PasswordHashSalt);

            securityData.HashedUsername = UserAuthenticator.
                                          CreateAuthenticationToken(this.UserName,
                                                                    securityData.UsernameHashSalt);

            this.SecurityData = securityData;
        }

        public static User NewUserWithFreshSalt(string newUserName, SecureString password)
        {
            return NewUserWithFreshSalt(newUserName, password.ConvertToUnsecureString());
        }

        public static User NewUserWithFreshSalt(string newUserName, string password)
        {
            var saltForUser = CryptoHashing.GenerateSalt(HashSaltSize);
            var saltedUsername = UserAuthenticator.CreateAuthenticationToken(newUserName, saltForUser);

            var saltForPassword = CryptoHashing.GenerateSalt(HashSaltSize);
            var saltedPassword = UserAuthenticator.CreateAuthenticationToken(password, saltForPassword);

            var saltForEncrKey = CryptoHashing.GenerateSalt(HashSaltSize);
            var saltForMacKey = CryptoHashing.GenerateSalt(HashSaltSize);

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
