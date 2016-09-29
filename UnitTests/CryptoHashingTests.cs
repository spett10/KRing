using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRing.Core;
using KRing.Extensions;
using System.Security;


namespace UnitTests
{
    [TestClass]
    public class CryptoHashingTests
    {
        [TestMethod]
        public void ScryptEncoding_GivingCorrect_ShouldSucced()
        {
            var password = new SecureString();
            password.PopulateWithString("UggaBuggaSuperSecret");

            string hashedPassword = CryptoHashing.ScryptHashPassword(password);

            bool isCorrectPassword = CryptoHashing.ScryptCheckPassword(password, hashedPassword);

            Assert.IsTrue(isCorrectPassword);
        }

        [TestMethod]
        public void ScryptEncoding_GivingIncorrect_ShouldFail()
        {
            var password = new SecureString();
            password.PopulateWithString("UggaBuggaSuperSecret");

            string hashedPassword = CryptoHashing.ScryptHashPassword(password);

            var wrongPassword = new SecureString();
            wrongPassword.PopulateWithString("UggaBuggaSuperSecret1");

            bool isCorrectPassword = CryptoHashing.ScryptCheckPassword(wrongPassword, hashedPassword);

            Assert.IsFalse(isCorrectPassword);
        }
    }
}
