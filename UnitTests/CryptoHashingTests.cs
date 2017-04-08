using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRing.Core;
using KRing.Extensions;
using System.Security;
using System.Diagnostics;
using System.Text;
using System.Security.Cryptography;

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

        [TestMethod]
        public void AesGCM_GiveCorrectKeyAndIv_ShouldBeEqual()
        {
            var plaintext = "Foo Bar Baz";
            var rawPlaintext = Encoding.UTF8.GetBytes(plaintext);

            var key = Encoding.UTF8.GetBytes("YELLOW SUBMARINEYELLOW SUBMARINE");
            var iv = CryptoHashing.GenerateSalt(12);

            var cipher = Aes256AuthenticatedCipher.Encrypt(rawPlaintext, key, iv);

            var decryptedRaw = Aes256AuthenticatedCipher.Decrypt(cipher, key, iv);

            var plaintextAfterDecryption = Encoding.UTF8.GetString(decryptedRaw);

            Assert.IsTrue(plaintext.Equals(plaintextAfterDecryption));
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void AesGCM_FiddleWithData_ShouldThrowException()
        {
            var plaintext = "Foo Bar Baz";
            var rawPlaintext = Encoding.UTF8.GetBytes(plaintext);

            var key = Encoding.UTF8.GetBytes("YELLOW SUBMARINEYELLOW SUBMARINE");
            var iv = CryptoHashing.GenerateSalt(12);

            var cipher = Aes256AuthenticatedCipher.Encrypt(rawPlaintext, key, iv);

            /* FIDDLE */
            cipher.ciphertext[0] ^= byte.MaxValue;

            var decryptedRaw = Aes256AuthenticatedCipher.Decrypt(cipher, key, iv);
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void AesGCM_FiddleWithTag_ShouldThrowException()
        {
            var plaintext = "Foo Bar Baz";
            var rawPlaintext = Encoding.UTF8.GetBytes(plaintext);

            var key = Encoding.UTF8.GetBytes("YELLOW SUBMARINEYELLOW SUBMARINE");
            var iv = CryptoHashing.GenerateSalt(12);

            var cipher = Aes256AuthenticatedCipher.Encrypt(rawPlaintext, key, iv);

            /* FIDDLE */
            cipher.tag[0] ^= byte.MaxValue;

            var decryptedRaw = Aes256AuthenticatedCipher.Decrypt(cipher, key, iv);
        }
    }
}
