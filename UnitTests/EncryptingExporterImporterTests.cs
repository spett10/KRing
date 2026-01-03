using KRingCore.Core.Services;
using KRingCore.Krypto;
using KRingCore.Persistence.Interfaces;
using KRingCore.Persistence.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace UnitTests
{
    /// <summary>
    /// Summary description for EncryptingExporterImporterTests
    /// </summary>
    [TestClass]
    public class EncryptingExporterImporterTests
    {
        private List<StoredPassword> passwords;

        public EncryptingExporterImporterTests()
        {
            passwords = new List<StoredPassword>()
            {
                new StoredPassword("google", "bob", "abc123"),
                new StoredPassword("battlenet", "bo", "p4ssw0rd"),
                new StoredPassword("nsa backdoor", "bobo", "admin1")
            };
        }

        public TestContext TestContext { get; set; }


        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ExportThenImport_CorrectPasswordAndValidFilename_ShouldSuceed()
        {
            var securePassword = new SecureString();
            var password = "secret";
            foreach(var c in password)
            {
                securePassword.AppendChar(c);
            }

            var exporter = new EncryptingPasswordExporter(new KeyGenerator(), securePassword, deriveIterations: 1);
            var exported = exporter.ExportPasswords(this.passwords);

            Assert.IsFalse(string.IsNullOrEmpty(exported));

            var mockReader = new MockStreamReadToEnd(exported);

            var importer = new DecryptingPasswordImporter(new KeyGenerator(), securePassword, exportImportIterations: 1);
            var importedList = importer.ImportPasswords("test.txt", mockReader);

            Assert.IsNotNull(importedList);
            for(int i = 0; i < this.passwords.Count; i++)
            {
                var left = passwords.ElementAt(i);
                var right = importedList.ElementAt(i);

                Assert.AreEqual(right.Domain, left.Domain);
                Assert.AreEqual(right.PlaintextPassword, left.PlaintextPassword);
                Assert.AreEqual(right.Username, left.Username);
            }
        }

        [TestMethod]
        public void ExportThenImport_WrongPassword_ShouldThrowException()
        {
            var securePassword = new SecureString();
            var password = "secret";
            foreach (var c in password)
            {
                securePassword.AppendChar(c);
            }

            var exporter = new EncryptingPasswordExporter(new KeyGenerator(), securePassword, deriveIterations: 1);
            var exported = exporter.ExportPasswords(this.passwords);

            Assert.IsFalse(string.IsNullOrEmpty(exported));

            var mockReader = new MockStreamReadToEnd(exported);

            //Alter password so its wrong
            securePassword.AppendChar('e');

            var importer = new DecryptingPasswordImporter(new KeyGenerator(), securePassword, exportImportIterations: 1);
            Assert.Throws<CryptoHashing.IntegrityException>(() => importer.ImportPasswords("test.txt", mockReader));
        }

        class MockStreamReadToEnd : IStreamReadToEnd
        {
            private string _output;

            public MockStreamReadToEnd(string output)
            {
                _output = output;
            }

            public string ReadToEnd(string filename)
            {
                return _output;
            }

            public Task<string> ReadToEndAsync(string filename)
            {
                return Task.Run(() => { return _output; });
            }
        }
    }

    
}
