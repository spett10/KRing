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

            var password = new SecureString();

            password.PopulateWithString("YELLOW SUBMARINE");

            var passwordSalted = CryptoHashing.ScryptHashPassword(password);
            var saltForKey = CryptoHashing.GenerateSalt();
            var saltForHash = CryptoHashing.GenerateSalt();

            _correctUser = new User("testuser",
                                    password,
                                    new Cookie(passwordSalted, saltForKey, saltForHash));
        }

        public MockingProfileRepository(string username, SecureString password)
        {
            var passwordSalted = CryptoHashing.ScryptHashPassword(password);
            var saltForKey = CryptoHashing.GenerateSalt();
            var saltForHash = CryptoHashing.GenerateSalt();

            _correctUser = new User(username,
                                    password,
                                    new Cookie(passwordSalted, saltForKey, saltForHash));
            
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
