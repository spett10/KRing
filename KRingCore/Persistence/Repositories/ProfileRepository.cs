using System;
using System.IO;
using System.Security;
using KRingCore.Core.Model;
using KRingCore.Extensions;
using System.Configuration;
using KRingCore.Persistence.Interfaces;
using System.Threading.Tasks;
using KRingCore.Core;

namespace KRingCore.Persistence.Repositories
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
                profileWriter.WriteLine(Convert.ToBase64String(user.SecurityData.HashedUsername));
                profileWriter.WriteLine(Convert.ToBase64String(user.SecurityData.UsernameHashSalt));
                profileWriter.WriteLine(Convert.ToBase64String(user.SecurityData.HashedPassword));
                profileWriter.WriteLine(Convert.ToBase64String(user.SecurityData.PasswordHashSalt));
                
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
                await profileWriter.WriteLineAsync(Convert.ToBase64String(user.SecurityData.HashedUsername));
                await profileWriter.WriteLineAsync(Convert.ToBase64String(user.SecurityData.UsernameHashSalt));
                await profileWriter.WriteLineAsync(Convert.ToBase64String(user.SecurityData.HashedPassword));
                await profileWriter.WriteLineAsync(Convert.ToBase64String(user.SecurityData.PasswordHashSalt));
            }
        }

        public User ReadUser()
        {
            using (StreamReader profileReader = new StreamReader(_profilePath))
            {
                var storedUser = Convert.FromBase64String(profileReader.ReadLine());
                if (storedUser == null) { throw new IOException("Empty Profile"); }

                var storedUsernameSalt = profileReader.ReadLine();
                if(storedUsernameSalt == null) { throw new IOException("Empty Username Salt"); }
                var userSalt = Convert.FromBase64String(storedUsernameSalt);

                var storedPasswordSalted = Convert.FromBase64String(profileReader.ReadLine());
                if (storedPasswordSalted == null) { throw new IOException("Empty Password for profile"); }
                
                var storedHashSalt = profileReader.ReadLine();
                if (storedHashSalt == null) { throw new IOException("No salt for password"); }
                var hashSalt = Convert.FromBase64String(storedHashSalt);

                var cookie = new SecurityData(storedPasswordSalted, storedUser, hashSalt, userSalt);

                return new User(" ",cookie);
            }
        }

        public async Task<User> ReadUserAsync()
        {
            using (StreamReader profileReader = new StreamReader(_profilePath))
            {
                var storedUser = await profileReader.ReadLineAsync();
                if (storedUser == null) { throw new IOException("Empty Profile"); }

                var storedUsernameSalt = await profileReader.ReadLineAsync();
                if (storedUsernameSalt == null) { throw new IOException("Empty Username Salt"); }
                var userSalt = Convert.FromBase64String(storedUsernameSalt);

                var storedPasswordSalted = await profileReader.ReadLineAsync();
                if (storedPasswordSalted == null) { throw new IOException("Empty Password for profile"); }

                var storedHashSalt = await profileReader.ReadLineAsync();
                if (storedHashSalt == null) { throw new IOException("No salt for password"); }
                var hashSalt = Convert.FromBase64String(storedHashSalt);

                var cookie = new SecurityData(storedPasswordSalted, storedUser, hashSalt, userSalt);

                return new User(" ", cookie);
            }
        }
    }
}
