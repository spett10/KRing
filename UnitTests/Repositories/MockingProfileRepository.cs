using System;
using KRing.Persistence.Interfaces;
using KRing.Core.Model;
using System.Security;
using KRing.Core;
using KRing.Extensions;
using System.Diagnostics;
using System.Threading.Tasks;

namespace UnitTests
{
    public class MockingProfileRepository : IProfileRepository
    {
        public User _correctUser;
        
        public MockingProfileRepository()
        {
            var passwordRaw = "YELLOW SUBMARINE";
            var usernameRaw = "testuser";
            var password = new SecureString();

            password.PopulateWithString(passwordRaw);

            var passwordSalted = CryptoHashing.ScryptHashPassword(passwordRaw);
            var usernameSalted = CryptoHashing.ScryptHashPassword(usernameRaw);
            var saltForKey = CryptoHashing.GenerateSalt();
            var saltForHash = CryptoHashing.GenerateSalt();

            _correctUser = new User(usernameRaw,
                                    password,
                                    new SecurityData(passwordSalted, usernameSalted, saltForKey, saltForHash, saltForHash));
        }

        public MockingProfileRepository(string username, SecureString password)
        {
            var rawPass = password.ConvertToUnsecureString();
            password = new SecureString();
            password.PopulateWithString(rawPass);

            var saltForKey = CryptoHashing.GenerateSalt();
            var saltForHash = CryptoHashing.GenerateSalt(64);
            var passwordSalted = CryptoHashing.ScryptHashPassword(rawPass, saltForHash);
            var usernameSalted = CryptoHashing.ScryptHashPassword(username, saltForHash);

            _correctUser = new User(username,
                                    password,
                                    new SecurityData(passwordSalted, usernameSalted, saltForKey, saltForHash, saltForHash));
            
        }

        public void DeleteUser()
        {
            return;
        }

        public Task DeleteUserAsync()
        {
            throw new NotImplementedException();
        }

        public User ReadUser()
        {
            return _correctUser;
        }

        public Task<User> ReadUserAsync()
        {
            throw new NotImplementedException();
        }

        public void WriteUser(User user)
        {
            _correctUser = user;
        }

        public Task WriteUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
