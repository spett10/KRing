﻿using System.Security;
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

        public static User NewUserWithFreshSalt(string newUserName, SecureString password)
        {
            var saltForPassword = CryptoHashing.GenerateSalt();
            var saltedPassword = CryptoHashing.GenerateSaltedHash(password.ConvertToUnsecureString(), saltForPassword);
            var saltForKey = CryptoHashing.GenerateSalt();

            var cookie = new Cookie(saltedPassword, saltForPassword, saltForKey);
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