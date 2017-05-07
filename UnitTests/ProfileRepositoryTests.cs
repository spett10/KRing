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
            var password = "Super Secure";
            var keySalt = CryptoHashing.GenerateSalt();
            var hashSalt = CryptoHashing.GenerateSalt();

            var userSalted = CryptoHashing.GenerateSaltedHash(username, hashSalt);
            var passwordSalted = CryptoHashing.GenerateSaltedHash(password, hashSalt);
            

            var cookie = new SecurityData(passwordSalted, userSalted, keySalt, hashSalt, hashSalt);

            var securePassword = new SecureString();
            securePassword.PopulateWithString(password);

            _user = new User(username, securePassword, cookie);
        }

        [TestMethod]
        public void WriteUser_ThenRead_ShouldBeEqual()
        {
            var repository = new ProfileRepository();

            repository.WriteUser(_user);

            var readUser = repository.ReadUser();

            var password = "Super Secure";

            var isValidPassword = CryptoHashing.CompareSaltedHash(password, readUser.Cookie.PasswordHashSalt, readUser.Cookie.HashedPassword); //think when we read the users password is not the password but the hashed password
            Assert.IsTrue(isValidPassword);
            
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

            var securePassword = new SecureString();
            securePassword.PopulateWithString(_user.PlaintextPassword);

            string username = "Bob";
            var salt = CryptoHashing.GenerateSalt();
            var passwordSalted = CryptoHashing.GenerateSaltedHash(_user.PlaintextPassword, salt);
            var userSalted = CryptoHashing.GenerateSaltedHash(username, salt);
            var keySalt = CryptoHashing.GenerateSalt();
            var hashSalt = CryptoHashing.GenerateSalt();
            
            var otherCookie = new SecurityData(passwordSalted, userSalted, keySalt, hashSalt, hashSalt);
            var otherUser = new User(username, securePassword, _user.Cookie);

            repository.WriteUser(otherUser);

            var readUser = repository.ReadUser();

            Assert.AreEqual(otherUser.Cookie.HashedPassword, readUser.Cookie.HashedPassword);

            var keysaltIsEqual = CryptoHashing.CompareByteArrays(readUser.Cookie.KeySalt, otherUser.Cookie.KeySalt);
            Assert.AreEqual(keysaltIsEqual, true);

            Assert.AreEqual(readUser.Cookie.HashedPassword, otherUser.Cookie.HashedPassword);
        }
    }
}
