using System;
using System.IO;
using System.Security;
using KRing.Core.Model;
using KRing.Extensions;
using System.Transactions;
using System.Configuration;
using KRing.Persistence.Interfaces;
using System.Threading.Tasks;
using KRing.Core;

namespace KRing.Persistence.Repositories
{
    public class ProfileRepository : ReleasePathDependent, IProfileRepository
    {
        private readonly string _profilePath;

        public ProfileRepository()
        {
#if DEBUG
            var filename = ConfigurationManager.AppSettings["relativeprofilePathDebug"];
            _profilePath = filename;
#else
            _profilePath = base.ReleasePathPrefix() + ConfigurationManager.AppSettings["relativeprofilePath"];
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

        public async Task DeleteUserAsync()
        {
            await FileUtil.FilePurgeAsync(_profilePath, "");
        }

        public void WriteUser(User user)
        {
            if(user == null)
                throw new ArgumentNullException();

            //Make sure we dont write two users
            DeleteUser();
            
            using (StreamWriter profileWriter = new StreamWriter(_profilePath))
            {
                profileWriter.WriteLine(user.Cookie.HashedUsername);
                profileWriter.WriteLine(Convert.ToBase64String(user.Cookie.UsernameHashSalt));
                profileWriter.WriteLine(user.Cookie.HashedPassword);
                profileWriter.WriteLine(Convert.ToBase64String(user.Cookie.PasswordHashSalt));
                profileWriter.WriteLine(Convert.ToBase64String(user.Cookie.KeySalt));
                
            }
        }

        public async Task WriteUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException();

            //Make sure we dont write two users
            await DeleteUserAsync();

            using (StreamWriter profileWriter = new StreamWriter(_profilePath))
            {
                await profileWriter.WriteLineAsync(user.Cookie.HashedUsername);
                await profileWriter.WriteLineAsync(Convert.ToBase64String(user.Cookie.UsernameHashSalt));
                await profileWriter.WriteLineAsync(user.Cookie.HashedPassword);
                await profileWriter.WriteLineAsync(Convert.ToBase64String(user.Cookie.PasswordHashSalt));
                await profileWriter.WriteLineAsync(Convert.ToBase64String(user.Cookie.KeySalt));
            }
        }

        public User ReadUser()
        {
            using (StreamReader profileReader = new StreamReader(_profilePath))
            {
                var storedUser = profileReader.ReadLine();
                if (storedUser == null) { throw new ArgumentNullException("Empty Profile"); }

                var storedUsernameSalt = profileReader.ReadLine();
                if(storedUsernameSalt == null) { throw new ArgumentNullException("Empty Username Salt"); }
                var userSalt = Convert.FromBase64String(storedUsernameSalt);

                var storedPasswordSalted = profileReader.ReadLine();
                if (storedPasswordSalted == null) { throw new ArgumentNullException("Empty Password for profile"); }

                
                var storedHashSalt = profileReader.ReadLine();
                if (storedHashSalt == null) { throw new ArgumentNullException("No salt for password"); }
                var hashSalt = Convert.FromBase64String(storedHashSalt);

                var storedKeySalt = profileReader.ReadLine();
                if (storedKeySalt == null) { throw new ArgumentNullException("No salt for key"); }
                var keySalt = Convert.FromBase64String(storedKeySalt);

                var cookie = new SecurityData(storedPasswordSalted, storedUser, keySalt, hashSalt, userSalt);

                return new User(" ",cookie);
            }
        }

        public async Task<User> ReadUserAsync()
        {
            using (StreamReader profileReader = new StreamReader(_profilePath))
            {
                var storedUser = await profileReader.ReadLineAsync();
                if (storedUser == null) { throw new ArgumentNullException("Empty Profile"); }

                var storedUsernameSalt = await profileReader.ReadLineAsync();
                if (storedUsernameSalt == null) { throw new ArgumentNullException("Empty Username Salt"); }
                var userSalt = Convert.FromBase64String(storedUsernameSalt);

                var storedPasswordSalted = await profileReader.ReadLineAsync();
                if (storedPasswordSalted == null) { throw new ArgumentNullException("Empty Password for profile"); }

                var storedHashSalt = await profileReader.ReadLineAsync();
                if (storedHashSalt == null) { throw new ArgumentNullException("No salt for password"); }
                var hashSalt = Convert.FromBase64String(storedHashSalt);

                var storedKeySalt = await profileReader.ReadLineAsync();
                if (storedKeySalt == null) { throw new ArgumentNullException("No salt for key"); }
                var keySalt = Convert.FromBase64String(storedKeySalt);                

                var cookie = new SecurityData(storedPasswordSalted, storedUser, keySalt, hashSalt, userSalt);

                return new User(" ", cookie);
            }
        }
    }
}
