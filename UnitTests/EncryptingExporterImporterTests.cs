﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRingCore.Persistence.Model;
using System.Security;
using KRingCore.Core.Services;
using KRingCore.Persistence.Interfaces;
using System.Linq;
using System.Security.Cryptography;

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

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

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

            var exporter = new EncryptingPasswordExporter();
            var exported = exporter.ExportPasswords(this.passwords, securePassword);

            Assert.IsTrue(!string.IsNullOrEmpty(exported));

            var mockReader = new MockStreamReadToEnd(exported);

            var importer = new DecryptingPasswordImporter();
            var importedList = importer.ImportPasswords("test.txt", securePassword, mockReader);

            Assert.IsTrue(importedList != null);
            for(int i = 0; i < this.passwords.Count; i++)
            {
                Assert.IsTrue(this.passwords.ElementAt(i) == importedList.ElementAt(i));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void ExportThenImport_WrongPassword_ShouldThrowException()
        {
            var securePassword = new SecureString();
            var password = "secret";
            foreach (var c in password)
            {
                securePassword.AppendChar(c);
            }

            var exporter = new EncryptingPasswordExporter();
            var exported = exporter.ExportPasswords(this.passwords, securePassword);

            Assert.IsTrue(!string.IsNullOrEmpty(exported));

            var mockReader = new MockStreamReadToEnd(exported);

            //Alter password so its wrong
            securePassword.AppendChar('e');

            var importer = new DecryptingPasswordImporter();
            var importedList = importer.ImportPasswords("test.txt", securePassword, mockReader);
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
        }
    }

    
}
