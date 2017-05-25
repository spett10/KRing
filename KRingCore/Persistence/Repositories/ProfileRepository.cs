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
                profileWriter.WriteLine(user.Cookie.HashedUsername);
                profileWriter.WriteLine(Convert.ToBase64String(user.Cookie.UsernameHashSalt));
                profileWriter.WriteLine(user.Cookie.HashedPassword);
                profileWriter.WriteLine(Convert.ToBase64String(user.Cookie.PasswordHashSalt));
                profileWriter.WriteLine(Convert.ToBase64String(user.Cookie.EncryptionKeySalt));
                profileWriter.WriteLine(Convert.ToBase64String(user.Cookie.MacKeySalt));
                
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
                await profileWriter.WriteLineAsync(Convert.ToBase64String(user.Cookie.EncryptionKeySalt));
                await profileWriter.WriteLineAsync(Convert.ToBase64String(user.Cookie.MacKeySalt));
            }
        }

        public User ReadUser()
        {
            using (StreamReader profileReader = new StreamReader(_profilePath))
            {
                var storedUser = profileReader.ReadLine();
                if (storedUser == null) { throw new IOException("Empty Profile"); }

                var storedUsernameSalt = profileReader.ReadLine();
                if(storedUsernameSalt == null) { throw new IOException("Empty Username Salt"); }
                var userSalt = Convert.FromBase64String(storedUsernameSalt);

                var storedPasswordSalted = profileReader.ReadLine();
                if (storedPasswordSalted == null) { throw new IOException("Empty Password for profile"); }

                
                var storedHashSalt = profileReader.ReadLine();
                if (storedHashSalt == null) { throw new IOException("No salt for password"); }
                var hashSalt = Convert.FromBase64String(storedHashSalt);

                var storedEncrKeySalt = profileReader.ReadLine();
                if (storedEncrKeySalt == null) { throw new IOException("No salt for encryption key"); }
                var encrKeySalt = Convert.FromBase64String(storedEncrKeySalt);

                var storedMacKeySalt = profileReader.ReadLine();
                if(storedMacKeySalt == null) { throw new IOException("No salt for mac key"); }
                var macKeySalt = Convert.FromBase64String(storedMacKeySalt);

                var cookie = new SecurityData(storedPasswordSalted, storedUser, encrKeySalt, macKeySalt, hashSalt, userSalt);

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

                var storedEncrKeySalt = await profileReader.ReadLineAsync();
                if (storedEncrKeySalt == null) { throw new IOException("No salt for key"); }
                var encrKeySalt = Convert.FromBase64String(storedEncrKeySalt);

                var storedMacKeySalt = await profileReader.ReadLineAsync();
                if (storedMacKeySalt == null) { throw new IOException("No salt for mac key"); }
                var macKeySalt = Convert.FromBase64String(storedMacKeySalt);

                var cookie = new SecurityData(storedPasswordSalted, storedUser, encrKeySalt, macKeySalt, hashSalt, userSalt);

                return new User(" ", cookie);
            }
        }
    }
}
