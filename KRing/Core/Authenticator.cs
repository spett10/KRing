using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using KRing.Extensions;

namespace KRing.Core
{
    public static class Authenticator
    {
        private static readonly string profilePath = "..\\..\\Data\\profile.txt";
        public static readonly int SaltByteSize = 16;
        private static readonly int iterations = 1000;
        private static User user;


        public static int UserCount { get; private set; }

        static Authenticator()
        {
            UserCount = 1;
            
            try
            {
                LoadProfile();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public static Session LogIn(string username, SecureString password)
        {
            bool isPasswordCorrect = false;

            var storedSaltedPassword = user.Cookie.PasswordSalted;
            var givenPasswordWhenSalted = GenerateSaltedHash(password, user.Cookie.SaltForPassword);

            isPasswordCorrect = CompareByteArrays(storedSaltedPassword, givenPasswordWhenSalted);

            bool isCorrectUser = username.Equals(user.UserName, StringComparison.Ordinal);

            bool allowLogin = isPasswordCorrect && isCorrectUser;

            if (allowLogin)
            {
                user.Login();
                return new Session(user);
            }
            else
            {
                throw new UnauthorizedAccessException("Username and/or password was incorrect");
            }
        }

        static void SaveProfile(User user)
        {
            using (StreamWriter profileWriter = new StreamWriter(profilePath))
            {
                profileWriter.WriteLine(user.UserName);
                profileWriter.WriteLine(Convert.ToBase64String(user.Cookie.PasswordSalted));
                profileWriter.WriteLine(Convert.ToBase64String(user.Cookie.SaltForPassword));
            }
        }

        /* todo: the securstring password the user gets here is the hashed password. */
        public static void LoadProfile()
        {
            using (StreamReader profileReader = new StreamReader(profilePath))
            {
                var storedUser = profileReader.ReadLine();
                if(storedUser == null) { throw new ArgumentNullException("Empty Profile");}
                
                var storedPasswordSalted = profileReader.ReadLine();
                if(storedPasswordSalted == null) { throw new ArgumentNullException("Empty Password for profile");}
                var passwordSalted = Convert.FromBase64String(storedPasswordSalted);

                var storedSalt = profileReader.ReadLine();
                if(storedSalt == null) { throw new ArgumentNullException("No salt for user");}
                var salt = Convert.FromBase64String(storedSalt);

                /* todo: read keysalt when its there (saved) */


                var securePassword = new SecureString();
                securePassword.PopulateWithString(storedPasswordSalted);

                var cookie = new Cookie(passwordSalted, salt, Authenticator.GenerateSalt());

                user = new User(storedUser, 
                                false, 
                                securePassword,
                                cookie);
            }
        }

        static byte[] GenerateSaltedHash(string plaintext, byte[] salt)
        {
            Rfc2898DeriveBytes algorithm = new Rfc2898DeriveBytes(plaintext, salt, iterations);
            try
            {
                return algorithm.GetBytes(plaintext.Length + salt.Length);
            }
            finally
            {
                algorithm.Dispose();
            }

        }

        static byte[] GenerateSaltedHash(SecureString plaintext, byte[] salt)
        {
            Rfc2898DeriveBytes algorithm = new Rfc2898DeriveBytes(plaintext.ConvertToUnsecureString(), salt, iterations);
            try
            {
                return algorithm.GetBytes(plaintext.Length + salt.Length);
            }
            finally
            {
                algorithm.Dispose();
            }
        }

        private static byte[] GenerateSalt(int size)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[size];
            try
            {
                rng.GetBytes(buffer);

                return buffer;
            }
            finally
            {
                rng.Dispose();
            }
        }

        public static byte[] GenerateSalt()
        {
            return GenerateSalt(SaltByteSize);
        }

        private static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            return array1.Length == array2.Length && !array1.Where((t, i) => t != array2[i]).Any();
        }

    }
}
