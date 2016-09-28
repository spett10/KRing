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

            var salt = CryptoHashing.GenerateSalt();
            var passwordSalted = CryptoHashing.GenerateSaltedHash(password, salt);
            var keySalt = CryptoHashing.GenerateSalt();

            var cookie = new Cookie(passwordSalted, salt, keySalt);

            _user = new User(username, password, cookie);
        }

        [TestMethod]
        public void WriteUser_ThenRead_ShouldBeEqual()
        {
            var repository = new ProfileRepository();

            repository.WriteUser(_user);

            var readUser = repository.ReadUser();

            Assert.AreEqual(readUser.UserName, _user.UserName);

            Assert.AreEqual(Convert.ToBase64String(_user.Cookie.PasswordSalted), readUser.Password.ConvertToUnsecureString());

            var keysaltIsEqual = CryptoHashing.CompareByteArrays(readUser.Cookie.KeySalt, _user.Cookie.KeySalt);
            Assert.AreEqual(keysaltIsEqual, true);

            var passwordSaltedIsEqual = CryptoHashing.CompareByteArrays(readUser.Cookie.PasswordSalted, _user.Cookie.PasswordSalted);
            Assert.AreEqual(passwordSaltedIsEqual, true);

            var saltForPasswordIsEqual = CryptoHashing.CompareByteArrays(readUser.Cookie.SaltForPassword, _user.Cookie.SaltForPassword);
            Assert.AreEqual(passwordSaltedIsEqual, true);
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

            
            var otherCookie = new Cookie(passwordSalted, salt, keySalt);
            var otherUser = new User("Bob", _user.Password, _user.Cookie);

            repository.WriteUser(otherUser);

            var readUser = repository.ReadUser();

            Assert.AreEqual(otherUser.UserName, readUser.UserName);

            Assert.AreEqual(Convert.ToBase64String(otherUser.Cookie.PasswordSalted), readUser.Password.ConvertToUnsecureString());

            var keysaltIsEqual = CryptoHashing.CompareByteArrays(readUser.Cookie.KeySalt, otherUser.Cookie.KeySalt);
            Assert.AreEqual(keysaltIsEqual, true);

            var passwordSaltedIsEqual = CryptoHashing.CompareByteArrays(readUser.Cookie.PasswordSalted, otherUser.Cookie.PasswordSalted);
            Assert.AreEqual(passwordSaltedIsEqual, true);

            var saltForPasswordIsEqual = CryptoHashing.CompareByteArrays(readUser.Cookie.SaltForPassword, otherUser.Cookie.SaltForPassword);
            Assert.AreEqual(passwordSaltedIsEqual, true);
        }
    }
}
