using KRingCore.Core.Model;
using KRingCore.Core.Services;
using KRingCore.Krypto;
using KRingCore.Krypto.Extensions;
using KRingCore.Persistence.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Security;

namespace UnitTests
{
    [TestClass]
    public class ProfileRepositoryTests
    {
        User _user;

        [TestInitialize]
        public void TestInitialize()
        {
            string username = "Alice";
            var password = "Super Secure";
            var encrKeySalt = CryptoHashing.GenerateSalt();
            var macKeySalt = CryptoHashing.GenerateSalt();
            var hashSalt = CryptoHashing.GenerateSalt();

            var userSalted = UserAuthenticator.CreateAuthenticationToken(username, hashSalt);
            var passwordSalted = UserAuthenticator.CreateAuthenticationToken(password, hashSalt);
            

            var cookie = new SecurityData(passwordSalted, userSalted, hashSalt, hashSalt);

            var securePassword = new SecureString();
            securePassword.PopulateWithString(password);

            _user = new User(username, securePassword, cookie);
        }

        [TestMethod]
        public void ReadUser_WithoutWritingFirst_ShouldThrowError()
        {
            var repository = new ProfileRepository();

            repository.DeleteUser();

            Assert.Throws<System.IO.IOException>(() => repository.ReadUser());
        }

        [TestMethod]
        public void WriteNullUser_ShouldThrowError()
        {
            var repository = new ProfileRepository();

            User user = null;

            Assert.Throws<ArgumentNullException>(() => repository.WriteUser(user));
        }

        [TestMethod]
        public void WriteUser_DeleteUser_ReadUser_ShouldThrowError()
        {
            var repository = new ProfileRepository();

            repository.WriteUser(_user);

            repository.DeleteUser();

            Assert.Throws<System.IO.IOException>(() => repository.ReadUser());
        }

        [TestMethod]
        public void WriteUser_WriteAnotherUser_ShouldReadLatest()
        {
            var repository = new ProfileRepository();

            repository.WriteUser(_user);

            var securePassword = new SecureString();
            securePassword.PopulateWithString(_user.PlaintextPassword);

            string username = "Bob";
            var salt = CryptoHashing.GenerateSalt();
            var passwordSalted = UserAuthenticator.CreateAuthenticationToken(_user.PlaintextPassword, salt);
            var userSalted = UserAuthenticator.CreateAuthenticationToken(username, salt);
            var encrKeySalt = CryptoHashing.GenerateSalt();
            var macKeySalt = CryptoHashing.GenerateSalt();
            var hashSalt = CryptoHashing.GenerateSalt();
            
            var otherCookie = new SecurityData(passwordSalted, userSalted, hashSalt, hashSalt);
            var otherUser = new User(username, securePassword, _user.SecurityData);

            repository.WriteUser(otherUser);

            var readUser = repository.ReadUser();

            Assert.IsTrue(otherUser.SecurityData.HashedPassword.SequenceEqual(readUser.SecurityData.HashedPassword));
                        
            Assert.IsTrue(readUser.SecurityData.HashedPassword.SequenceEqual(otherUser.SecurityData.HashedPassword));
        }
    }
}
