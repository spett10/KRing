using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRingCore.Persistence.Repositories;
using Krypto.Extensions;
using KRingCore.Interfaces;
using KRingCore.Persistence.Model;
using System.Security;
using UnitTests.Config;
using System.Configuration;
using Krypto;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class DBEntryRepositoryTests
    {
        SecureString _password;
        string _plaintextPassword = "YELLOW SUBMARINE";
        string _correctDomain;
        IDataConfig _config;

        [TestInitialize]
        public void TestInitialize()
        {
            _password = new SecureString();
            _password.PopulateWithString(_plaintextPassword);

            _correctDomain = "test";

            _config = new MockingDataConfig(ConfigurationManager.AppSettings["relativemetaPathDebug"],
                                            ConfigurationManager.AppSettings["relativedbPathDebug"],
                                            ConfigurationManager.AppSettings["relativeconfigPathDebug"]);
        }

        [TestMethod]
        public void CreateDBRepository()
        {
            var repository = new StoredPasswordRepository(_password, CryptoHashing.GenerateSalt(), CryptoHashing.GenerateSalt(), _config);

            Assert.AreEqual(repository.EntryCount, 0);
        }

        [TestMethod]
        public void AddEntry_ExistsEntry_ShouldExist()
        {
            var repository = new StoredPasswordRepository(_password, CryptoHashing.GenerateSalt(), CryptoHashing.GenerateSalt(), _config);

            repository.AddEntry(new StoredPassword(_correctDomain, _plaintextPassword));

            var exists = repository.ExistsEntry(_correctDomain);

            repository.DeleteAllEntries();

            Assert.AreEqual(exists, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Error: Domain Already Exists")]
        public void AddEntry_AddAgain_ShouldThrowException()
        {
            var repository = new StoredPasswordRepository(_password, CryptoHashing.GenerateSalt(), CryptoHashing.GenerateSalt(), _config);

            repository.AddEntry(new StoredPassword(_correctDomain, _plaintextPassword));

            //duplicates are not allowed
            repository.AddEntry(new StoredPassword(_correctDomain, _plaintextPassword));
        }

        [TestMethod]
        public void AddEntryThenDeleteIt_ExistsEntry_ShouldNotExist()
        {
            var repository = new StoredPasswordRepository(_password, CryptoHashing.GenerateSalt(), CryptoHashing.GenerateSalt(), _config);

            repository.AddEntry(new StoredPassword(_correctDomain, _plaintextPassword));
            repository.DeleteEntry(_correctDomain);

            var exists = repository.ExistsEntry(_correctDomain);

            Assert.AreEqual(exists, false);
        }

        [TestMethod]
        public void AddSomeEntries_DeleteAll_CheckExists()
        {
            var repository = new StoredPasswordRepository(_password, CryptoHashing.GenerateSalt(), CryptoHashing.GenerateSalt(), _config);

            string domain1 = "FooBar";
            string domain2 = "FooBaz";
            string domain3 = "Testing";

            repository.AddEntry(new StoredPassword(domain1, _plaintextPassword));
            repository.AddEntry(new StoredPassword(domain2, _plaintextPassword));
            repository.AddEntry(new StoredPassword(domain3, _plaintextPassword));

            var countDuring = repository.EntryCount;

            Assert.AreEqual(countDuring, 3);

            repository.DeleteAllEntries();

            var existsDomain1 = repository.ExistsEntry(domain1);
            var existsDomain2 = repository.ExistsEntry(domain2);
            var existsDomain3 = repository.ExistsEntry(domain3);

            var didAnyExist = existsDomain1 || existsDomain2 || existsDomain3;

            Assert.AreEqual(didAnyExist, false);

            var countAfter = repository.EntryCount;

            Assert.AreEqual(countAfter, 0);
        }

        [TestMethod]
        public void AddEntriesSaveDBNewDBLoadDB_DoEntriesExist_ShouldExist()
        {
            var encrHash = CryptoHashing.GenerateSalt();
            var macHash = CryptoHashing.GenerateSalt();
            var repository = new StoredPasswordRepository(_password, encrHash, macHash, _config);

            string domain1 = "FooBar";
            string domain2 = "FooBaz";
            string domain3 = "Testing";

            string username = "John Doe";

            repository.AddEntry(new StoredPassword(domain1, username, _plaintextPassword));
            repository.AddEntry(new StoredPassword(domain2, username, _plaintextPassword));
            repository.AddEntry(new StoredPassword(domain3, username, _plaintextPassword));

            repository.WriteEntriesToDb();

            //load db into new object, with same password. 
            var otherRepository = new StoredPasswordRepository(_password, encrHash, macHash, _config);
            otherRepository.LoadEntriesFromDb();

            var existsDomain1 = otherRepository.ExistsEntry(domain1);
            var existsDomain2 = otherRepository.ExistsEntry(domain2);
            var existsDomain3 = otherRepository.ExistsEntry(domain3);

            var didAllExist = existsDomain1 && existsDomain2 && existsDomain3;

            //we have to delete entries, to alter the config file st. there are 0 entries. Otherwise other tests will try to load the db.
            //and our tests should be independant.
            otherRepository.DeleteAllEntries(); 

            Assert.AreEqual(didAllExist, true);
        }

        [TestMethod]
        public void AddEntryThenDelete_ShouldNotExist()
        {
            var repository = new StoredPasswordRepository(_password, CryptoHashing.GenerateSalt(), CryptoHashing.GenerateSalt(), _config);

            repository.AddEntry(new StoredPassword(_correctDomain, _plaintextPassword));

            repository.DeleteEntry(_correctDomain);

            var result = repository.ExistsEntry(_correctDomain);

            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void AddEntries_DeleteByIndex_ShouldSucceed()
        {
            var repository = new StoredPasswordRepository(_password, CryptoHashing.GenerateSalt(), CryptoHashing.GenerateSalt(), _config);
            string fake_domain = "other";

            repository.AddEntry(new StoredPassword(_correctDomain, _plaintextPassword));
            repository.AddEntry(new StoredPassword(fake_domain, _plaintextPassword));
            repository.DeleteEntry(0);

            var result = repository.ExistsEntry(_correctDomain);
            Assert.AreEqual(result, false);

            repository.DeleteEntry(0);
            result = repository.ExistsEntry(fake_domain);
            Assert.AreEqual(result, false);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "domain does not exist to delete")]
        public void AddEntry_DeleteTwice_ShouldFail()
        {
            var repository = new StoredPasswordRepository(_password, CryptoHashing.GenerateSalt(), CryptoHashing.GenerateSalt(), _config);

            repository.AddEntry(new StoredPassword(_correctDomain, _plaintextPassword));

            repository.DeleteEntry(_correctDomain);

            //this should fail.
            repository.DeleteEntry(_correctDomain);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddEntry_DeleteWrongIndex_ShouldFail()
        {
            var repository = new StoredPasswordRepository(_password, CryptoHashing.GenerateSalt(), CryptoHashing.GenerateSalt(), _config);

            repository.AddEntry(new StoredPassword(_correctDomain, _plaintextPassword));

            repository.DeleteEntry(10);
        }

        [TestMethod]
        public void UpdateEntry_ExistsAlready_ShouldSuceed()
        {
            var repository = new StoredPasswordRepository(_password, CryptoHashing.GenerateSalt(), CryptoHashing.GenerateSalt(), _config);

            repository.AddEntry(new StoredPassword(_correctDomain, _plaintextPassword));

            var newPassword = "TESTING";
            var newDto = new StoredPassword(_correctDomain, newPassword);

            repository.UpdateEntry(newDto);

            var updatedPassword = repository.GetPasswordFromDomain(_correctDomain);

            Assert.AreEqual(updatedPassword, newPassword);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateEntry_EntryDoesNotExist_ShouldFail()
        {
            var repository = new StoredPasswordRepository(_password, CryptoHashing.GenerateSalt(), CryptoHashing.GenerateSalt(), _config);

            repository.AddEntry(new StoredPassword(_correctDomain, _plaintextPassword));

            var newDto = new StoredPassword("UGGA BUGGA", _plaintextPassword);

            repository.UpdateEntry(newDto);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetPasswordFromDomain_WrongDomain_ShouldReturnNull()
        {
            var repository = new StoredPasswordRepository(_password, CryptoHashing.GenerateSalt(), CryptoHashing.GenerateSalt(), _config);

            repository.AddEntry(new StoredPassword(_correctDomain, _plaintextPassword));

            var password = repository.GetPasswordFromDomain(_correctDomain + "not");
        }

        [TestMethod]
        public void PrefixSearch_OkScenario()
        {
            var repository = new StoredPasswordRepository(_password, CryptoHashing.GenerateSalt(), CryptoHashing.GenerateSalt(), _config);

            var dummyPassword = new char[12];
            var dummyUsername = "John Doe";

            var passwords = new List<StoredPassword>
            {
                new StoredPassword("Google", dummyUsername, dummyPassword),
                new StoredPassword("Goog", dummyUsername, dummyPassword),
                new StoredPassword("Glog", dummyUsername, dummyPassword),
                new StoredPassword("Facebook", dummyUsername, dummyPassword)
            };

            foreach(var pswd in passwords)
            {
                repository.AddEntry(pswd);
            }

            var result = repository.PrefixSearch("Goog");

            var success = result.Count == 2 && result.Exists(e => e.Domain == "Goog") && result.Exists(e => e.Domain == "Google");

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void PrefixSearch_NoMatches_ShouldReturnEmptyList()
        {
            var repository = new StoredPasswordRepository(_password, CryptoHashing.GenerateSalt(), CryptoHashing.GenerateSalt(), _config);

            var dummyPassword = new char[12];
            var dummyUsername = "John Doe";

            var passwords = new List<StoredPassword>
            {
                new StoredPassword("Google", dummyUsername, dummyPassword),
                new StoredPassword("Goog", dummyUsername, dummyPassword),
                new StoredPassword("Glog", dummyUsername, dummyPassword),
                new StoredPassword("Facebook",  dummyUsername, dummyPassword)
            };

            foreach (var pswd in passwords)
            {
                repository.AddEntry(pswd);
            }

            var result = repository.PrefixSearch("Amazon");

            var success = result.Count == 0;

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void GetEntry_StringOverload_ShouldReturnCorrect()
        {
            var repository = new StoredPasswordRepository(_password, CryptoHashing.GenerateSalt(), CryptoHashing.GenerateSalt(), _config);

            var dummyPassword = new char[12];
            var dummyUsername = "John Doe";

            var passwords = new List<StoredPassword>
            {
                new StoredPassword("Google", "Jane", dummyPassword),
                new StoredPassword("Goog", dummyUsername, dummyPassword),
                new StoredPassword("Glog", dummyUsername, dummyPassword),
                new StoredPassword("Facebook",  dummyUsername, dummyPassword)
            };

            foreach (var pswd in passwords)
            {
                repository.AddEntry(pswd);
            }

            var result = repository.GetEntry("Google");

            var success = result.Domain == "Google" && result.Username == "Jane";

            Assert.IsTrue(success);
        }

    }
}
