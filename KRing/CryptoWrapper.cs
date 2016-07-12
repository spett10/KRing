using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;

namespace KRing
{
    public static class CryptoWrapper
    {
        private static readonly int keysize_bits = 128;
        private static readonly int blocksize_bits = 128;
        private static readonly int keysize_bytes = 16;
        private static readonly int blocksize_bytes = 16;

        public static byte[] ECB_Encrypt(byte[] data, byte[] key)
        {
            return ECB_Encrypt(data, key, Enumerable.Repeat((byte)0, CryptoWrapper.keysize_bytes).ToArray());
        }

        private static byte[] ECB_Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            var aesAlg = new AesManaged
            {
                KeySize = keysize_bits,
                Key = key,
                BlockSize = blocksize_bits,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.Zeros,
                IV = iv
            };

            var encrypted = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV)
                .TransformFinalBlock(data, 0, data.Length);
            return encrypted;

        }

        public static byte[] ECB_Decrypt(byte[] data, byte[] key)
        {
            return ECB_Decrypt(data, key, Enumerable.Repeat((byte)0, CryptoWrapper.keysize_bytes).ToArray());
        }

        public static byte[] ECB_Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            var aesAlg = new AesManaged
            {
                KeySize = keysize_bits,
                Key = key,
                BlockSize = blocksize_bits,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.Zeros,
                IV = iv
            };

            var decrypted = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV)
                .TransformFinalBlock(data, 0, data.Length);
            return decrypted;
        }

        public static byte[] CBC_Encrypt(byte[] data, byte[] key)
        {
            return CBC_Encrypt(data, key, Enumerable.Repeat((byte)0, CryptoWrapper.keysize_bytes).ToArray());
        }

        public static byte[] CBC_Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            if (key.Length < keysize_bytes || iv.Length < keysize_bytes) throw new ArgumentException("Keysize or IV too small, must be 16 bytes");
            if (key.Length != keysize_bytes) throw new ArgumentException("Keysize must be 16 bytes");

            int data_length = data.Length;
            int rounds = (int)Math.Ceiling((float)data_length / (float)blocksize_bytes);
            int padding_needed = data_length % blocksize_bytes;

            /* do padding if needed */
            byte[] plaintext = data;
            if (padding_needed != 0)
            {
                plaintext = PKCSHashtag7Padding(plaintext, blocksize_bytes);
            }

            /* handle first block with IV explicitly */
            byte[] first_block = new byte[blocksize_bytes];
            for (int j = 0; j < blocksize_bytes; j++)
            {
                first_block[j] = plaintext[j];
            }

            first_block = RepeatingKeyXOR(first_block, iv);
            byte[] cipher_block = ECB_Encrypt(first_block, key);
            byte[] ciphertext = new byte[plaintext.Length];
            for (int j = 0; j < blocksize_bytes; j++)
            {
                ciphertext[j] = cipher_block[j];
            }

            /* take previous cipher block, next plaintext block and XOR together. */
            /* Take result and feed to AES with the key. Write the resulting block to ciphertext, and keep going */
            for (int i = 1; i < rounds; i++)
            {
                byte[] temp = new byte[blocksize_bytes];

                for (int j = 0; j < blocksize_bytes; j++)
                {
                    temp[j] = plaintext[i * blocksize_bytes + j];
                }

                temp = RepeatingKeyXOR(temp, cipher_block);
                cipher_block = ECB_Encrypt(temp, key);

                for (int j = 0; j < blocksize_bytes; j++)
                {
                    ciphertext[i * blocksize_bytes + j] = cipher_block[j];
                }

            }

            return ciphertext;

        }

        public static byte[] CBC_Decrypt(byte[] data, byte[] key)
        {
            return CBC_Decrypt(data, key, Enumerable.Repeat((byte)0, CryptoWrapper.keysize_bytes).ToArray());
        }

        public static byte[] CBC_Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            int data_length = data.Length;
            int rounds = (int)Math.Ceiling((float)data_length / (float)blocksize_bytes);

            byte[] plaintext = new byte[data.Length];
            byte[] ciphertext = data;

            /* handle first block explicitly with IV */
            byte[] temp = new byte[blocksize_bytes];
            byte[] cipher_block = new byte[blocksize_bytes];
            for (int j = 0; j < blocksize_bytes; j++)
            {
                cipher_block[j] = ciphertext[j];
            }

            temp = ECB_Decrypt(cipher_block, key);
            temp = RepeatingKeyXOR(temp, iv);

            for (int j = 0; j < blocksize_bytes; j++)
            {
                plaintext[j] = temp[j];
            }

            /* now iterate (could do this in parallel actually) */
            byte[] old_cipher_block = new byte[blocksize_bytes];
            for (int i = 1; i < rounds; i++)
            {
                for (int j = 0; j < blocksize_bytes; j++)
                {
                    old_cipher_block[j] = cipher_block[j];
                    cipher_block[j] = ciphertext[i * blocksize_bytes + j];
                }

                temp = ECB_Decrypt(cipher_block, key);
                temp = RepeatingKeyXOR(temp, old_cipher_block);

                for (int j = 0; j < blocksize_bytes; j++)
                {
                    plaintext[i * blocksize_bytes + j] = temp[j];
                }
            }

            return plaintext;
        }

        public static string PKCSHashtag7Padding(string ascii, int blocksize)
        {
            string data_hex = Converter.FromStringToHex(ascii).Replace("-", string.Empty);
            byte[] data_raw = Converter.FromHexToBase64Byte(data_hex);
            byte[] result = PKCSHashtag7Padding(data_raw, blocksize);
            return Encoding.ASCII.GetString(result);
        }

        public static byte[] PKCSHashtag7Padding(byte[] data, int blocksize_bytes)
        {
            int max_blocksize = 256;
            if (blocksize_bytes > max_blocksize) throw new ArgumentException("blocksize must be less than 256");

            int bytes_to_pad = blocksize_bytes - (data.Length % blocksize_bytes);
            Console.WriteLine("bytes to pad {0}", bytes_to_pad);
            int padded_length = data.Length + bytes_to_pad;
            byte[] padded = new byte[padded_length];

            for (int i = 0; i < data.Length; i++)
            {
                padded[i] = data[i];
            }

            for (int i = data.Length; i < padded_length; i++)
            {
                padded[i] = (byte)bytes_to_pad;
            }

            return padded;
        }

        public static string PKCSHashtag7Validation(string data, int blocksize)
        {
            return Encoding.ASCII.GetString(PKCSHashtag7Validation(Encoding.ASCII.GetBytes(data), blocksize));
        }

        /* the min padding will be 1 */
        /* the last byte should denote how many bytes we should strip off. strip them, and check that they are all equal */
        public static byte[] PKCSHashtag7Validation(byte[] data, int blocksize)
        {
            int max = blocksize - 1;
            int min = 1;

            int padding_amount = (int)data[data.Length - 1];

            bool is_number = (padding_amount <= max) && (padding_amount >= min);
            if (!is_number) throw new ArgumentException("Invalid Padding");

            byte[] padding = data.Skip(data.Length - padding_amount).Take(padding_amount).ToArray();
            byte first = padding[0];
            for (int i = 1; i < padding.Length; i++)
            {
                byte next = padding[i];
                if (!first.Equals(next)) throw new ArgumentException("Invalid Padding");
            }

            byte[] stripped = data.Take(data.Length - padding_amount).ToArray();

            return stripped;
        }

        public static byte[] RepeatingKeyXOR(string ascii_buffer, string ascii_key)
        {
            string hex_buffer = Converter.FromStringToHex(ascii_buffer);
            byte[] raw_buffer = Converter.FromHexToBase64Byte(hex_buffer);
            byte[] XORd = new byte[raw_buffer.Length];

            for (int i = 0; i < XORd.Length; i++)
            {
                char roundkey = ascii_key[i % ascii_key.Length];
                XORd[i] = (byte)(raw_buffer[i] ^ Convert.ToByte(roundkey));
            }

            return XORd;
        }

        public static byte[] RepeatingKeyXOR(byte[] buffer, byte[] repeating_key)
        {
            byte[] XORd = new byte[buffer.Length];
            for (int i = 0; i < buffer.Length; i++)
            {
                XORd[i] = (byte)(buffer[i] ^ repeating_key[i % repeating_key.Length]);
            }
            return XORd;
        }


    }
}
