using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using KRing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class EncryptionTests
    {
        [TestMethod]
        public void EncryptionDecryptionToFile_CorrectKeyAndIV_ShouldGiveSamePlaintext()
        {
            //Setup
            var password = Encoding.ASCII.GetBytes("Yellow Submarine");
            var expected = "Super Secret Message";
            var plaintext = Encoding.ASCII.GetBytes(expected);

            var IV = Authenticator.GenerateSalt();


            //Write
            RijndaelManaged rmEncryptor = new RijndaelManaged();
            FileStream fileStreamWrite = new FileStream("..\\..\\testlog.txt", FileMode.Create);

            CryptoStream encryptStream = new CryptoStream(fileStreamWrite, rmEncryptor.CreateEncryptor(password, IV), CryptoStreamMode.Write);

            StreamWriter streamWriter = new StreamWriter(encryptStream);

            streamWriter.WriteLine(expected);

            streamWriter.Close();
            encryptStream.Close();
            fileStreamWrite.Close();

            //Read
            RijndaelManaged rmDecryptor = new RijndaelManaged();
            FileStream fileStreamRead = new FileStream("..\\..\\testlog.txt", FileMode.Open);

            CryptoStream decryptStream = new CryptoStream(fileStreamRead, rmDecryptor.CreateDecryptor(password, IV), CryptoStreamMode.Read);

            StreamReader streamReader = new StreamReader(decryptStream);

            var result = streamReader.ReadLine();

            streamReader.Close();
            decryptStream.Close();
            fileStreamRead.Close();

            //Assertion
            Assert.AreEqual(result, expected, true, result + " should be equal to " + expected);
        }

        [TestMethod]
        public void EncryptionDecryptionToFileWithIVAlsoStored_CorrectKeyAndIV_ShouldBeEqaul()
        {
            //Setup
            var password = Encoding.ASCII.GetBytes("Yellow Submarine");
            var expected = "Super Secret Message";
            var plaintext = Encoding.ASCII.GetBytes(expected);

            var IV = Authenticator.GenerateSalt();


            //Write Encrypted Text
            RijndaelManaged rmEncryptor = new RijndaelManaged();
            FileStream fileStreamWrite = new FileStream("..\\..\\testlog.txt", FileMode.Create);

            CryptoStream encryptStream = new CryptoStream(fileStreamWrite, rmEncryptor.CreateEncryptor(password, IV), CryptoStreamMode.Write);

            StreamWriter streamWriter = new StreamWriter(encryptStream);

            streamWriter.WriteLine(expected);
            streamWriter.WriteLine(expected);

            streamWriter.Close();
            encryptStream.Close();
            
            fileStreamWrite.Close();

            //Read IV
            FileStream fileStreamRead = new FileStream("..\\..\\testlog.txt", FileMode.Open);
            RijndaelManaged rmDecryptor = new RijndaelManaged();
            CryptoStream decryptStream = new CryptoStream(
                                            fileStreamRead, 
                                            rmDecryptor.CreateDecryptor(
                                                password, 
                                                IV),
                                            CryptoStreamMode.Read);

            StreamReader streamReader = new StreamReader(decryptStream);

            var result = streamReader.ReadLine();

            Assert.AreEqual(result, expected, true, result + " should be equal to " + expected);
        }

    }
}
