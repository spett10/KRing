using System;
using KRingCore.Persistence.Interfaces;
using KRingCore.Core.Model;
using System.Security;
using Krypto.Extensions;
using System.Threading.Tasks;
using Krypto;
using KRingCore.Core.Services;

namespace UnitTests
{
    public class MockingProfileRepository : IProfileRepository
    {
        public User _correctUser;
        
        public MockingProfileRepository()
        {
            var passwordRaw = "YELLOW SUBMARINE";
            var usernameRaw = "testuser";
            var saltForEncrKey = CryptoHashing.GenerateSalt();
            var saltForMacKey = CryptoHashing.GenerateSalt();
            var saltForHash = CryptoHashing.GenerateSalt();
            var password = new SecureString();

            password.PopulateWithString(passwordRaw);

            var passwordSalted = UserAuthenticator.CreateAuthenticationToken(passwordRaw, saltForHash);
            var usernameSalted = UserAuthenticator.CreateAuthenticationToken(usernameRaw, saltForHash);            

            _correctUser = new User(usernameRaw,
                                    password,
                                    new SecurityData(passwordSalted, usernameSalted, saltForHash, saltForHash));
        }

        public MockingProfileRepository(string username, SecureString password)
        {
            var rawPass = password.ConvertToUnsecureString();
            password = new SecureString();
            password.PopulateWithString(rawPass);

            var saltForEncrKey = CryptoHashing.GenerateSalt();
            var saltForMacKey = CryptoHashing.GenerateSalt();
            var saltForHash = CryptoHashing.GenerateSalt(64);
            var passwordSalted = UserAuthenticator.CreateAuthenticationToken(rawPass, saltForHash);
            var usernameSalted = UserAuthenticator.CreateAuthenticationToken(username, saltForHash);

            _correctUser = new User(username,
                                    password,
                                    new SecurityData(passwordSalted, usernameSalted, saltForHash, saltForHash));
            
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
