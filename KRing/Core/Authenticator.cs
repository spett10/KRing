using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using KRing.Extensions;
using KRing.Persistence;

namespace KRing.Core
{
    public class Authenticator
    {
        public int UserCount { get; private set; }

        private readonly string profilePath = "..\\..\\Data\\profile.txt";
        private User _user;

        public Authenticator()
        {
            UserCount = 1;
        }

        public Session LogIn(string username, SecureString password)
        {
            bool isPasswordCorrect = false;

            var storedSaltedPassword = _user.Cookie.PasswordSalted;
            var givenPasswordWhenSalted = CryptoHashing.GenerateSaltedHash(password, _user.Cookie.SaltForPassword);

            isPasswordCorrect = CryptoHashing.CompareByteArrays(storedSaltedPassword, givenPasswordWhenSalted);

            bool isCorrectUser = username.Equals(_user.UserName, StringComparison.Ordinal);

            bool allowLogin = isPasswordCorrect && isCorrectUser;

            if (allowLogin)
            {
                _user.Login();
                _user.Password = password;
                return new Session(_user);
            }
            else
            {
                throw new UnauthorizedAccessException("Username and/or password was incorrect");
            }
        }

        public void NewProfile(User user)
        {
            if(user == null) Console.WriteLine("User not good");

            using (StreamWriter profileWriter = new StreamWriter(profilePath))
            {
                profileWriter.WriteLine(user.UserName);
                profileWriter.WriteLine(Convert.ToBase64String(user.Cookie.PasswordSalted));
                profileWriter.WriteLine(Convert.ToBase64String(user.Cookie.SaltForPassword));
                profileWriter.WriteLine(Convert.ToBase64String(user.Cookie.KeySalt));
            }

            LoadProfile();
        }

        public void DeleteProfile()
        {
            FileUtil.FilePurge(profilePath, "");
        }

        public void LoadProfile()
        {
            using (StreamReader profileReader = new StreamReader(profilePath))
            {
                var storedUser = profileReader.ReadLine();
                if(storedUser == null) { throw new ArgumentNullException("Empty Profile");}
                
                var storedPasswordSalted = profileReader.ReadLine();
                if(storedPasswordSalted == null) { throw new ArgumentNullException("Empty Password for profile");}
                var passwordSalted = Convert.FromBase64String(storedPasswordSalted);

                var storedSalt = profileReader.ReadLine();
                if(storedSalt == null) { throw new ArgumentNullException("No salt for _user");}
                var salt = Convert.FromBase64String(storedSalt);
                
                var storedKeySalt = profileReader.ReadLine();
                if(storedKeySalt == null) { throw new ArgumentNullException("No salt for key");}
                var keySalt = Convert.FromBase64String(storedKeySalt);

                var securePassword = new SecureString();
                securePassword.PopulateWithString(storedPasswordSalted);

                var cookie = new Cookie(passwordSalted, salt, keySalt);

                _user = new User(storedUser, 
                                false, 
                                securePassword,
                                cookie);
            }
        }

        

    }
}
