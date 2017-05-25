using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRingCore.Core;
using System.Text;
using System.Security.Cryptography;

namespace UnitTests
{
    [TestClass]
    public class CryptoHashingTests
    {
        [TestMethod]
        public void PBKDF2_GivingCorrect_ShouldSucced()
        {
            var password = "UggaBuggaSuperSecret";
            var salt = CryptoHashing.GenerateSalt();

            var hashedPassword = Convert.ToBase64String(CryptoHashing.GenerateSaltedHash(password, salt));

            bool isCorrectPassword = CryptoHashing.CompareSaltedHash(password, salt, hashedPassword);

            Assert.IsTrue(isCorrectPassword);
        }

        [TestMethod]
        public void PBKDF2_GivingIncorrect_ShouldFail()
        {
            var password = "UggaBuggaSuperSecret";
            var salt = CryptoHashing.GenerateSalt();

            var hashedPassword = Convert.ToBase64String(CryptoHashing.GenerateSaltedHash(password, salt));

            var wrongpassword = "UggaBuggaSuperSecret1";

            bool isCorrectPassword = CryptoHashing.CompareSaltedHash(wrongpassword, salt, hashedPassword);

            Assert.IsFalse(isCorrectPassword);
        }

        [TestMethod]
        public void CBCThenHMAC_GiveCorrectKeyAndIv_ShouldBeEqual()
        {
            var plaintext = "Foo Bar Baz";
            var rawPlaintext = Encoding.UTF8.GetBytes(plaintext);

            var encrkey = Encoding.UTF8.GetBytes("YELLOW SUBMARINEYELLOW SUBMARINE");
            var hmacKEy = Encoding.UTF8.GetBytes("MELLOW SUBMARINEMELLOW SUBMARINE");
            var iv = CryptoHashing.GenerateSalt();

            var cipher = Aes256AuthenticatedCipher.CBCEncryptThenHMac(rawPlaintext, iv, encrkey, hmacKEy);

            var decryptedRaw = Aes256AuthenticatedCipher.VerifyMacThenCBCDecrypt(cipher, encrkey, iv, hmacKEy);

            var plaintextAfterDecryption = Encoding.UTF8.GetString(decryptedRaw);

            Assert.IsTrue(plaintext.Equals(plaintextAfterDecryption));
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void CBCThenHMAC_FiddleWithCipher_ShouldThrowException()
        {
            var plaintext = "Foo Bar Baz";
            var rawPlaintext = Encoding.UTF8.GetBytes(plaintext);

            var key = Encoding.UTF8.GetBytes("YELLOW SUBMARINEYELLOW SUBMARINE");
            var hmacKEy = Encoding.UTF8.GetBytes("MELLOW SUBMARINEMELLOW SUBMARINE");
            var iv = CryptoHashing.GenerateSalt();

            var cipher = Aes256AuthenticatedCipher.CBCEncryptThenHMac(rawPlaintext, key, iv, hmacKEy);

            /* FIDDLE */
            cipher.ciphertext[0] ^= byte.MaxValue;

            var decryptedRaw = Aes256AuthenticatedCipher.VerifyMacThenCBCDecrypt(cipher, key, iv, hmacKEy);
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void CBCThenHMAC_FiddleWithTag_ShouldThrowException()
        {
            var plaintext = "Foo Bar Baz";
            var rawPlaintext = Encoding.UTF8.GetBytes(plaintext);

            var key = Encoding.UTF8.GetBytes("YELLOW SUBMARINEYELLOW SUBMARINE");
            var hmacKEy = Encoding.UTF8.GetBytes("MELLOW SUBMARINEMELLOW SUBMARINE");
            var iv = CryptoHashing.GenerateSalt();

            var cipher = Aes256AuthenticatedCipher.CBCEncryptThenHMac(rawPlaintext, key, iv, hmacKEy);

            /* FIDDLE */
            cipher.tag[0] ^= byte.MaxValue;

            var decryptedRaw = Aes256AuthenticatedCipher.VerifyMacThenCBCDecrypt(cipher, key, iv, hmacKEy);
        }


        //[TestMethod]
        //public void AesGCM_GiveCorrectKeyAndIv_ShouldBeEqual()
        //{
        //    var plaintext = "Foo Bar Baz";
        //    var rawPlaintext = Encoding.UTF8.GetBytes(plaintext);

        //    var key = Encoding.UTF8.GetBytes("YELLOW SUBMARINEYELLOW SUBMARINE");
        //    var iv = CryptoHashing.GenerateSalt(12);

        //    var cipher = Aes256AuthenticatedCipher.Encrypt(rawPlaintext, key, iv);

        //    var decryptedRaw = Aes256AuthenticatedCipher.Decrypt(cipher, key, iv);

        //    var plaintextAfterDecryption = Encoding.UTF8.GetString(decryptedRaw);

        //    Assert.IsTrue(plaintext.Equals(plaintextAfterDecryption));
        //}

        //[TestMethod]
        //[ExpectedException(typeof(CryptographicException))]
        //public void AesGCM_FiddleWithData_ShouldThrowException()
        //{
        //    var plaintext = "Foo Bar Baz";
        //    var rawPlaintext = Encoding.UTF8.GetBytes(plaintext);

        //    var key = Encoding.UTF8.GetBytes("YELLOW SUBMARINEYELLOW SUBMARINE");
        //    var iv = CryptoHashing.GenerateSalt(12);

        //    var cipher = Aes256AuthenticatedCipher.Encrypt(rawPlaintext, key, iv);

        //    /* FIDDLE */
        //    cipher.ciphertext[0] ^= byte.MaxValue;

        //    var decryptedRaw = Aes256AuthenticatedCipher.Decrypt(cipher, key, iv);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(CryptographicException))]
        //public void AesGCM_FiddleWithTag_ShouldThrowException()
        //{
        //    var plaintext = "Foo Bar Baz";
        //    var rawPlaintext = Encoding.UTF8.GetBytes(plaintext);

        //    var key = Encoding.UTF8.GetBytes("YELLOW SUBMARINEYELLOW SUBMARINE");
        //    var iv = CryptoHashing.GenerateSalt(12);

        //    var cipher = Aes256AuthenticatedCipher.Encrypt(rawPlaintext, key, iv);

        //    /* FIDDLE */
        //    cipher.tag[0] ^= byte.MaxValue;

        //    var decryptedRaw = Aes256AuthenticatedCipher.Decrypt(cipher, key, iv);
        //}
    }
}
