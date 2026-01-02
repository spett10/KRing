using KRingCore.Core.Model;
using KRingCore.Core.Services;
using KRingCore.Krypto;
using KRingCore.Krypto.Extensions;
using KRingCore.Persistence.Interfaces;
using System;
using System.Security;
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
            var saltForEncrKey = CryptoHashing.GenerateSalt();
            var saltForMacKey = CryptoHashing.GenerateSalt();
            var saltForHash = CryptoHashing.GenerateSalt();
            var password = new SecureString();

            password.PopulateWithString(passwordRaw);

            var userAuthenticator = new UserAuthenticator(loginIterations: 1);
            var passwordSalted = userAuthenticator.CreateAuthenticationToken(passwordRaw, saltForHash);
            var usernameSalted = userAuthenticator.CreateAuthenticationToken(usernameRaw, saltForHash);            

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

            var userAuthenticator = new UserAuthenticator(loginIterations: 1);

            var passwordSalted = userAuthenticator.CreateAuthenticationToken(rawPass, saltForHash);
            var usernameSalted = userAuthenticator.CreateAuthenticationToken(username, saltForHash);

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
