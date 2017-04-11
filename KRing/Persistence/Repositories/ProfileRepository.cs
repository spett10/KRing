using System;
using System.IO;
using System.Security;
using KRing.Core.Model;
using KRing.Extensions;
using System.Transactions;
using System.Configuration;
using KRing.Persistence.Interfaces;

namespace KRing.Persistence.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly string _profilePath;

        public ProfileRepository()
        {
#if DEBUG
            var filename = ConfigurationManager.AppSettings["relativeprofilePathDebug"];
            _profilePath = filename;
#else
            var filename = ConfigurationManager.AppSettings["relativeprofilePath"];
            _profilePath = Environment.CurrentDirectory + filename;
#endif

        }

        public ProfileRepository(string profilePath)
        {
            this._profilePath = profilePath;
        }

        public void DeleteUser()
        {
            FileUtil.FilePurge(_profilePath, "");
        }

        /* TODO: encrypt the salted password */
        public void WriteUser(User user)
        {
            if(user == null)
                throw new ArgumentNullException();

            //Make sure we dont write two users
            DeleteUser();
            
            using (StreamWriter profileWriter = new StreamWriter(_profilePath))
            {
                profileWriter.WriteLine(user.UserName);
                profileWriter.WriteLine(user.Cookie.HashedPassword);
                profileWriter.WriteLine(Convert.ToBase64String(user.Cookie.KeySalt));
                profileWriter.WriteLine(Convert.ToBase64String(user.Cookie.HashSalt));
            }

        }

        public User ReadUser()
        {
            using (StreamReader profileReader = new StreamReader(_profilePath))
            {
                var storedUser = profileReader.ReadLine();
                if (storedUser == null) { throw new ArgumentNullException("Empty Profile"); }

                var storedPasswordSalted = profileReader.ReadLine();
                if (storedPasswordSalted == null) { throw new ArgumentNullException("Empty Password for profile"); }

                var storedKeySalt = profileReader.ReadLine();
                if (storedKeySalt == null) { throw new ArgumentNullException("No salt for key"); }
                var keySalt = Convert.FromBase64String(storedKeySalt);

                var storedHashSalt = profileReader.ReadLine();
                if (storedHashSalt == null) { throw new ArgumentNullException("No salt for password"); }
                var hashSalt = Convert.FromBase64String(storedHashSalt);

                var cookie = new SecurityData(storedPasswordSalted, keySalt, hashSalt);

                return new User(storedUser,cookie);
            }
        }
    }
}
