using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace KRing
{
    public static class Authenticator
    {
        private static readonly string ProfilePath = "..\\..\\Data\\profile.txt";
        private static readonly int SaltByteSize = 16;
        private static readonly int Iterations = 1000;


        public static int UserCount { get; private set; }

        static Authenticator()
        {
            UserCount = 1;
        }

        public static Session LogIn(string username, string password)
        {
            string StoredUser;
            bool IsPasswordCorrect = false;

            using (StreamReader profile = new StreamReader(ProfilePath))
            {
                StoredUser = profile.ReadLine();
                byte[] storedPasswordSalted = Convert.FromBase64String(profile.ReadLine());
                byte[] storedSalt = Convert.FromBase64String(profile.ReadLine());

                byte[] givenPasswordSalted = GenerateSaltedHash(password, storedSalt);

                IsPasswordCorrect = CompareByteArrays(storedPasswordSalted, givenPasswordSalted);
            }

            bool IsCorrectUser = username.Equals(StoredUser, StringComparison.Ordinal);

            bool AllowLogin = IsPasswordCorrect && IsCorrectUser;

            return new Session(new User(username, AllowLogin));
        }


        /* TODO: move to fileutiliyclass */
        private static void LineChanger(string newText, string filePath, int lineToEdit)
        {
            string[] allLines = File.ReadAllLines(filePath);
            allLines[allLines.Length - 1] = newText;
            File.WriteAllLines(filePath, allLines);
        }


        static byte[] GenerateSaltedHash(string plaintext, byte[] salt)
        {
            Rfc2898DeriveBytes algorithm = new Rfc2898DeriveBytes(plaintext, salt, Iterations);
            return algorithm.GetBytes(plaintext.Length + salt.Length);
        }

        private static byte[] GenerateSalt(int size)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[size];
            rng.GetBytes(buffer);

            return buffer;
        }

        private static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            return array1.Length == array2.Length && !array1.Where((t, i) => t != array2[i]).Any();
        }

    }
}
