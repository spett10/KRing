﻿using System;
using System.IO;
using System.Security.Cryptography;
using static Krypto.Cipher.AesHmacAuthenticatedCipher;

namespace Krypto.Cipher
{
    /// <summary>
    /// Base class for an authenticated cipher. Needs a symmetric encryption algorithm and a keyed hash algorithm. 
    /// Uses CBC and PKCS7 mode by default. The idea is that we can extend it in the future, say, going to another HMAC
    /// if the used one is not good enough, or away from some symmetric algorithm. 
    /// 
    /// TODO: How do we extract the ciphermode and the padding? We cant use enum in constrants.. More delegates? There are just so many.
    /// Maybe an interface in, that has 3 delegates, one for ciphermode, one for padding, one for hmaccreation, then we only have one argument. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    public abstract class AuthenticatedCipher<T, V> where T : SymmetricAlgorithm, new() where V : KeyedHashAlgorithm
    {
        private readonly CipherMode _cipherMode;
        private readonly PaddingMode _paddingMode;

        public AuthenticatedCipher(CipherMode mode, PaddingMode padding)
        {
            _cipherMode = mode;
            _paddingMode = padding;
        }

        protected AuthenticatedCiphertext EncryptThenTag(byte[] plaintext, byte[] encrKey, byte[] iv, byte[] hmacKey, Func<byte[], V> keyedHashCreator)
        {
            if (CryptoHashing.CompareByteArraysNoTimeLeak(encrKey, hmacKey))
            {
                throw new ArgumentException("Using same key for encryption and mac is insecure");
            }

            var cipher = new AuthenticatedCiphertext();

            using (T symmetric = new T())
            {
                symmetric.Mode = _cipherMode;
                symmetric.Padding = _paddingMode;
                symmetric.Key = encrKey;
                symmetric.IV = iv;

                using (MemoryStream ms = new MemoryStream())
                using (var encryptor = symmetric.CreateEncryptor())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plaintext, 0, plaintext.Length);
                    cs.FlushFinalBlock();
                    cipher.ciphertext = ms.ToArray();
                }
            }

            using (V keyedHash = keyedHashCreator(hmacKey))
            {
                cipher.tag = keyedHash.ComputeHash(cipher.ciphertext);
            }

            return cipher;
        }

        protected byte[] VerifyThenDecrypt(AuthenticatedCiphertext ciphertext, byte[] encrKey, byte[] iv, byte[] hmacKey, Func<byte[], V> keyedHashCreator)
        {
            if (CryptoHashing.CompareByteArraysNoTimeLeak(encrKey, hmacKey))
            {
                throw new ArgumentException("Using same key for encryption and mac is insecure");
            }

            using (var hmac = keyedHashCreator(hmacKey))
            {
                var computedHash = hmac.ComputeHash(ciphertext.ciphertext);
                if (!CryptoHashing.CompareByteArraysNoTimeLeak(computedHash, ciphertext.tag))
                {
                    throw new CryptographicException("Invalid HMAC");
                }
            }

            using (T symmetric = new T())
            {
                symmetric.Mode = _cipherMode;
                symmetric.Padding = _paddingMode;
                symmetric.Key = encrKey;
                symmetric.IV = iv;

                using (MemoryStream ms = new MemoryStream())
                using (var decryptor = symmetric.CreateDecryptor())
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(ciphertext.ciphertext, 0, ciphertext.ciphertext.Length);
                    cs.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }
    }
}
