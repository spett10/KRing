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

            var saltForPassword = CryptoHashing.GenerateSalt();
            var passwordSalted = CryptoHashing.GenerateSaltedHash(password, saltForPassword);
            var saltForKey = CryptoHashing.GenerateSalt();

            _correctUser = new User("testuser",
                                    password,
                                    new Cookie(passwordSalted, saltForPassword, saltForKey));
        }

        public MockingProfileRepository(string username, SecureString password)
        {
            var saltForPassword = CryptoHashing.GenerateSalt();
            var passwordSalted = CryptoHashing.GenerateSaltedHash(password, saltForPassword);
            var saltForKey = CryptoHashing.GenerateSalt();

            _correctUser = new User(username,
                                    password,
                                    new Cookie(passwordSalted, saltForPassword, saltForKey));

            Debug.WriteLine("saltforpassword: " + Convert.ToBase64String(passwordSalted));
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
