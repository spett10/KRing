using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRingCore.Core.Model;
using KRingCore.Security;
using KRingCore.Extensions;
using KRingCore.Persistence.Repositories;
using System.Security;
using System.Linq;

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

            var userSalted = CryptoHashing.GenerateSaltedHash(username, hashSalt);
            var passwordSalted = CryptoHashing.GenerateSaltedHash(password, hashSalt);
            

            var cookie = new SecurityData(passwordSalted, userSalted, encrKeySalt, macKeySalt, hashSalt, hashSalt);

            var securePassword = new SecureString();
            securePassword.PopulateWithString(password);

            _user = new User(username, securePassword, cookie);
        }

        [TestMethod]
        [ExpectedException(typeof(System.IO.IOException))]
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
        [ExpectedException(typeof(System.IO.IOException))]
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
            var encrKeySalt = CryptoHashing.GenerateSalt();
            var macKeySalt = CryptoHashing.GenerateSalt();
            var hashSalt = CryptoHashing.GenerateSalt();
            
            var otherCookie = new SecurityData(passwordSalted, userSalted, encrKeySalt, macKeySalt, hashSalt, hashSalt);
            var otherUser = new User(username, securePassword, _user.SecurityData);

            repository.WriteUser(otherUser);

            var readUser = repository.ReadUser();

            Assert.IsTrue(otherUser.SecurityData.HashedPassword.SequenceEqual(readUser.SecurityData.HashedPassword));

            var keysaltIsEqual = CryptoHashing.CompareByteArraysNoTimeLeak(readUser.SecurityData.EncryptionKeySalt, otherUser.SecurityData.EncryptionKeySalt);
            Assert.AreEqual(keysaltIsEqual, true);

            Assert.IsTrue(readUser.SecurityData.HashedPassword.SequenceEqual(otherUser.SecurityData.HashedPassword));
        }
    }
}
