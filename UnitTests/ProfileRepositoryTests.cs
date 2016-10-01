using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRing.Core.Model;
using KRing.Core;
using KRing.Extensions;
using KRing.Persistence.Repositories;
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
            var password = new SecureString();
            password.PopulateWithString("Super Secure");

            var passwordSalted = CryptoHashing.ScryptHashPassword(password);
            var keySalt = CryptoHashing.GenerateSalt();

            var cookie = new Cookie(passwordSalted, keySalt);

            _user = new User(username, password, cookie);
        }

        [TestMethod]
        public void WriteUser_ThenRead_ShouldBeEqual()
        {
            var repository = new ProfileRepository();

            repository.WriteUser(_user);

            var readUser = repository.ReadUser();

            var password = new SecureString();
            password.PopulateWithString("Super Secure");

            var isValidPassword = CryptoHashing.ScryptCheckPassword(password, readUser.Cookie.HashedPassword); //think when we read the users password is not the password but the hashed password
            Assert.IsTrue(isValidPassword);

            Assert.AreEqual(readUser.UserName, _user.UserName);

            var keysaltIsEqual = CryptoHashing.CompareByteArrays(readUser.Cookie.KeySalt, _user.Cookie.KeySalt);
            Assert.AreEqual(keysaltIsEqual, true);
            
            Assert.AreEqual(_user.Cookie.HashedPassword, readUser.Cookie.HashedPassword);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadUser_WithoutWritingFirst_ShouldThrowError()
        {
            var repository = new ProfileRepository();

            repository.DeleteUser();

            var readUser = repository.ReadUser();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteNullUser_ShouldThrowError()
        {
            var repository = new ProfileRepository();

            User user = null;

            repository.WriteUser(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteUser_DeleteUser_ReadUser_ShouldThrowError()
        {
            var repository = new ProfileRepository();

            repository.WriteUser(_user);

            repository.DeleteUser();

            var user = repository.ReadUser();
        }

        [TestMethod]
        public void WriteUser_WriteAnotherUser_ShouldReadLatest()
        {
            var repository = new ProfileRepository();

            repository.WriteUser(_user);

            var salt = CryptoHashing.GenerateSalt();
            var passwordSalted = CryptoHashing.GenerateSaltedHash(_user.Password, salt);
            var keySalt = CryptoHashing.GenerateSalt();

            
            var otherCookie = new Cookie(passwordSalted, keySalt);
            var otherUser = new User("Bob", _user.Password, _user.Cookie);

            repository.WriteUser(otherUser);

            var readUser = repository.ReadUser();

            Assert.AreEqual(otherUser.UserName, readUser.UserName);

            Assert.AreEqual(otherUser.Cookie.HashedPassword, readUser.Cookie.HashedPassword);

            var keysaltIsEqual = CryptoHashing.CompareByteArrays(readUser.Cookie.KeySalt, otherUser.Cookie.KeySalt);
            Assert.AreEqual(keysaltIsEqual, true);

            Assert.AreEqual(readUser.Cookie.HashedPassword, otherUser.Cookie.HashedPassword);
        }
    }
}
