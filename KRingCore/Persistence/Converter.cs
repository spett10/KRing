using System;
using System.IO;

namespace KRingCore.Persistence
{
    public static class Converter
    {
        public static string Base64Encode(string plaintext)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plaintext);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }


        public static byte[] FromHexToBase64Byte(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] result = new byte[hex.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return result;
        }

        public static string FromHexToBase64String(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] result = new byte[hex.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return Convert.ToBase64String(result);
        }

        public static string FromStringToHex(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }

        public static byte[,] From1Dto2DbyteArray(int height, int width, byte[] source)
        {
            byte[,] res = new byte[height, width];

            for (int i = 0; i < height - 1; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    res[i, j] = source[i * width + j];
                }
            }

            /* handle the last block explicitly, it might not be alligned properly */
            /* maybe whatever is "left" in the last few entries are perturbing things? Could insert == which is ignored in base64`*/
            int lastblock = height - 1;
            for (int j = 0; j < source.Length % width; j++)
            {
                res[lastblock, j] = source[lastblock * width + j];
            }

            return res;
        }

        public static byte[,] Transpose2DByteArray(int height, int width, byte[,] source)
        {
            byte[,] res = new byte[width, height];
            for (int trans_blocks = 0; trans_blocks < width; trans_blocks++)
            {
                for (int blocks = 0; blocks < height; blocks++)
                {
                    res[trans_blocks, blocks] = source[blocks, trans_blocks];
                }
            }
            return res;
        }

        public static void Print2DByteArray(byte[,] input, int rows, int columns, System.IO.StreamWriter log)
        {
            log.WriteLine("Printing 2D array of {0} x {1} dim", rows, columns);
            byte[] temp = new byte[columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    temp[j] = input[i, j];
                }
                var str = Convert.ToBase64String(temp);
                log.WriteLine("row {0}: " + str.ToString(), i);
            }
        }

        public static byte[] Base64FileToByteArray(string filename)
        {
            string input = string.Empty;
            string line;
            StreamReader sr = new StreamReader(filename);
            try
            {
                while ((line = sr.ReadLine()) != null)
                {
                    input += line;
                }
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    ((IDisposable)sr).Dispose();
                }

            }

            return Convert.FromBase64String(input);
        }

    }
}
