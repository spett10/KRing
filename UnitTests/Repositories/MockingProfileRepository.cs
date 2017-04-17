using System;
using KRing.Persistence.Interfaces;
using KRing.Core.Model;
using System.Security;
using KRing.Core;
using KRing.Extensions;
using System.Diagnostics;

namespace UnitTests
{
    public class MockingProfileRepository : IProfileRepository
    {
        public User _correctUser;
        
        public MockingProfileRepository()
        {
            var passwordRaw = "YELLOW SUBMARINE";
            var password = new SecureString();

            password.PopulateWithString(passwordRaw);

            var passwordSalted = CryptoHashing.ScryptHashPassword(passwordRaw);
            var saltForKey = CryptoHashing.GenerateSalt();
            var saltForHash = CryptoHashing.GenerateSalt();

            _correctUser = new User("testuser",
                                    password,
                                    new SecurityData(passwordSalted, saltForKey, saltForHash));
        }

        public MockingProfileRepository(string username, SecureString password)
        {
            var rawPass = password.ConvertToUnsecureString();
            password = new SecureString();
            password.PopulateWithString(rawPass);

            var saltForKey = CryptoHashing.GenerateSalt();
            var saltForHash = CryptoHashing.GenerateSalt(64);
            var passwordSalted = CryptoHashing.ScryptHashPassword(rawPass, saltForHash);

            _correctUser = new User(username,
                                    password,
                                    new SecurityData(passwordSalted, saltForKey, saltForHash));
            
        }

        public void DeleteUser()
        {
            return;
        }

        public User ReadUser()
        {
            return _correctUser;
        }

        public void WriteUser(User user)
        {
            _correctUser = user;
        }
    }
}
