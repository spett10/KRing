using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Security.Cryptography;
using KRingCore.Security;
using System.Diagnostics;
using System.Linq;

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

            var crypto = new AesHmacAuthenticatedCipher(CipherMode.CBC, PaddingMode.PKCS7);

            var cipher = crypto.EncryptThenHMac(rawPlaintext, iv, encrkey, hmacKEy);

            var decryptedRaw = crypto.VerifyMacThenDecrypt(cipher, encrkey, iv, hmacKEy);

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

            var crypto = new AesHmacAuthenticatedCipher(CipherMode.CBC, PaddingMode.PKCS7);

            var cipher = crypto.EncryptThenHMac(rawPlaintext, key, iv, hmacKEy);

            /* FIDDLE */
            cipher.ciphertext[0] ^= byte.MaxValue;

            var decryptedRaw = crypto.VerifyMacThenDecrypt(cipher, key, iv, hmacKEy);
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

            var crypto = new AesHmacAuthenticatedCipher(CipherMode.CBC, PaddingMode.PKCS7);

            var cipher = crypto.EncryptThenHMac(rawPlaintext, key, iv, hmacKEy);

            /* FIDDLE */
            cipher.tag[0] ^= byte.MaxValue;

            var decryptedRaw = crypto.VerifyMacThenDecrypt(cipher, key, iv, hmacKEy);
        }

        [TestMethod]
        public void TestPBKDF2HMACSHA256_TestVectors()
        {
            //Test vector taken from RFC6070
            var plain = Encoding.ASCII.GetBytes("password");
            var salt = Encoding.ASCII.GetBytes("salt");
            var iterations = 1;
            var keylength = 20*8;

            var result = BitConverter.ToString(CryptoHashing.PBKDF2HMACSHA256(plain, salt, iterations, keylength));
            result = result.ToLowerInvariant().Replace("-", "");
            var expected = "120fb6cffcf8b32c43e7225256c4f837a86548c9";

            Debug.WriteLine(result);

            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void TestPBKDF2HMACSHA256_ExperimentalDeriveAppropriateTime()
        {
            // Used to time key derivation. 
            var plain = Encoding.ASCII.GetBytes("password");
            var salt = CryptoHashing.GenerateSalt(32);
            var iterations = 1048576;
            var keylength = 32 * 8;

            var result = BitConverter.ToString(CryptoHashing.PBKDF2HMACSHA256(plain, salt, iterations, keylength));
            result = result.ToLowerInvariant().Replace("-", "");
            Debug.WriteLine(result);
        }
    }
}
