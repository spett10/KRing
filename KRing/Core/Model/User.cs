﻿using System.Security;
using KRing.Extensions;

namespace KRing.Core.Model
{
    public class User
    {
        public string UserName { get; private set; }
        public bool IsLoggedIn { get; private set; }
        public SecureString Password { get; set; }
        public Cookie Cookie { get; private set; }

        public User(string name, bool loggedIn, SecureString password, Cookie cookie)
        {
            UserName = name;
            IsLoggedIn = loggedIn;
            Password = password;
            Cookie = cookie;
        }

        public static User NewUserWithFreshSalt(string newUserName, SecureString password)
        {
            var saltForPassword = CryptoHashing.GenerateSalt();
            var saltedPassword = CryptoHashing.GenerateSaltedHash(password.ConvertToUnsecureString(), saltForPassword);
            var saltForKey = CryptoHashing.GenerateSalt();

            var cookie = new Cookie(saltedPassword, saltForPassword, saltForKey);
            return new User(newUserName, false, password, cookie);
        }

        public static User DummyUser()
        {
            return new User("Dummy", false, new SecureString(),
                                    new Cookie(CryptoHashing.GenerateSalt(),
                                                CryptoHashing.GenerateSalt(),
                                                CryptoHashing.GenerateSalt()));
        }

        public void Login()
        {
            this.IsLoggedIn = true;
        }

        public void Logout()
        {
            this.IsLoggedIn = false;
        }

        public override string ToString()
        {
            return UserName;
        }

    }
}
