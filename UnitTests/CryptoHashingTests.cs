using KRingCore;
using KRingCore.Krypto;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UnitTests
{
    [TestClass]
    public class CryptoHashingTests
    {
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
            //Test vector taken from https://github.com/brycx/Test-Vector-Generation/blob/master/PBKDF2/pbkdf2-hmac-sha2-test-vectors.md since RFC6070 only has for SHA1 variant. 
            var plain = Encoding.ASCII.GetBytes("passwordPASSWORDpassword");
            var salt = Encoding.ASCII.GetBytes("saltSALTsaltSALTsaltSALTsaltSALTsalt");
            var iterations = 4096;
            var keylength = 25 * 8;

            var result = BitConverter.ToString(CryptoHashing.PBKDF2HMACSHA256(plain, salt, iterations, keylength));
            result = result.ToLowerInvariant().Replace("-", "");
            var expected = "348c89dbcbd32b2f32d814b8116e84cf2b17347ebc1800181c";

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
