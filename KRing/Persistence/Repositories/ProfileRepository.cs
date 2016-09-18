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
            var filename = ConfigurationManager.AppSettings["profilePathDebug"];
#else
            var filename = ConfigurationManager.AppSettings["profilePath"];
#endif

            _profilePath = filename;
        }

        public ProfileRepository(string profilePath)
        {
            this._profilePath = profilePath;
        }

        public void DeleteUser()
        {
            FileUtil.FilePurge(_profilePath, "");
        }

        public void WriteUser(User user)
        {
            if(user == null)
                throw new ArgumentNullException();

            using (TransactionScope scope = new TransactionScope())
            {
                using (StreamWriter profileWriter = new StreamWriter(_profilePath))
                {
                    profileWriter.WriteLine(user.UserName);
                    profileWriter.WriteLine(Convert.ToBase64String(user.Cookie.PasswordSalted));
                    profileWriter.WriteLine(Convert.ToBase64String(user.Cookie.SaltForPassword));
                    profileWriter.WriteLine(Convert.ToBase64String(user.Cookie.KeySalt));
                }

                scope.Complete();
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
                var passwordSalted = Convert.FromBase64String(storedPasswordSalted);

                var storedSalt = profileReader.ReadLine();
                if (storedSalt == null) { throw new ArgumentNullException("No salt for _user"); }
                var salt = Convert.FromBase64String(storedSalt);

                var storedKeySalt = profileReader.ReadLine();
                if (storedKeySalt == null) { throw new ArgumentNullException("No salt for key"); }
                var keySalt = Convert.FromBase64String(storedKeySalt);

                var securePassword = new SecureString();
                securePassword.PopulateWithString(storedPasswordSalted);

                var cookie = new Cookie(passwordSalted, salt, keySalt);

                return new User(storedUser,
                                securePassword, 
                                cookie);
            }
        }
    }
}
