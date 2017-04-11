using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRing.Persistence.Repositories;
using KRing.Extensions;
using KRing.Interfaces;
using KRing.Persistence.Model;
using System.Security;
using UnitTests.Config;
using System.Configuration;

namespace UnitTests
{
    [TestClass]
    public class DBEntryRepositoryTests
    {
        SecureString _password;
        string _correctDomain;
        IDataConfig _config;

        [TestInitialize]
        public void TestInitialize()
        {
            _password = new SecureString();
            _password.PopulateWithString("YELLOW SUBMARINE");

            _correctDomain = "test";

            _config = new MockingDataConfig(ConfigurationManager.AppSettings["metaPathDebug"],
                                            ConfigurationManager.AppSettings["dbPathDebug"],
                                            ConfigurationManager.AppSettings["configPathDebug"]);
        }

        [TestMethod]
        public void CreateDBRepository()
        {
            var repository = new StoredPasswordRepository(_password);

            Assert.AreEqual(repository.EntryCount, 0);
        }

        [TestMethod]
        public void AddEntry_ExistsEntry_ShouldExist()
        {
            var repository = new StoredPasswordRepository(_password);

            repository.AddEntry(new StoredPassword(_correctDomain, _password));

            var exists = repository.ExistsEntry(_correctDomain);

            repository.DeleteAllEntries();

            Assert.AreEqual(exists, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Error: Domain Already Exists")]
        public void AddEntry_AddAgain_ShouldThrowException()
        {
            var repository = new StoredPasswordRepository(_password);

            repository.AddEntry(new StoredPassword(_correctDomain, _password));

            //duplicates are not allowed
            repository.AddEntry(new StoredPassword(_correctDomain, _password));
        }

        [TestMethod]
        public void AddEntryThenDeleteIt_ExistsEntry_ShouldNotExist()
        {
            var repository = new StoredPasswordRepository(_password);

            repository.AddEntry(new StoredPassword(_correctDomain, _password));
            repository.DeleteEntry(_correctDomain);

            var exists = repository.ExistsEntry(_correctDomain);

            Assert.AreEqual(exists, false);
        }

        [TestMethod]
        public void AddSomeEntries_DeleteAll_CheckExists()
        {
            var repository = new StoredPasswordRepository(_password);

            string domain1 = "FooBar";
            string domain2 = "FooBaz";
            string domain3 = "Testing";

            repository.AddEntry(new StoredPassword(domain1, _password));
            repository.AddEntry(new StoredPassword(domain2, _password));
            repository.AddEntry(new StoredPassword(domain3, _password));

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
            var repository = new StoredPasswordRepository(_password);

            string domain1 = "FooBar";
            string domain2 = "FooBaz";
            string domain3 = "Testing";

            repository.AddEntry(new StoredPassword(domain1, _password));
            repository.AddEntry(new StoredPassword(domain2, _password));
            repository.AddEntry(new StoredPassword(domain3, _password));

            repository.WriteEntriesToDb();

            //load db into new object, with same password. 
            var otherRepository = new StoredPasswordRepository(_password);
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
            var repository = new StoredPasswordRepository(_password);

            repository.AddEntry(new StoredPassword(_correctDomain, _password));

            repository.DeleteEntry(_correctDomain);

            var result = repository.ExistsEntry(_correctDomain);

            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void AddEntries_DeleteByIndex_ShouldSucceed()
        {
            var repository = new StoredPasswordRepository(_password);
            string fake_domain = "other";

            repository.AddEntry(new StoredPassword(_correctDomain, _password));
            repository.AddEntry(new StoredPassword(fake_domain, _password));
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
            var repository = new StoredPasswordRepository(_password);

            repository.AddEntry(new StoredPassword(_correctDomain, _password));

            repository.DeleteEntry(_correctDomain);

            //this should fail.
            repository.DeleteEntry(_correctDomain);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddEntry_DeleteWrongIndex_ShouldFail()
        {
            var repository = new StoredPasswordRepository(_password);

            repository.AddEntry(new StoredPassword(_correctDomain, _password));

            repository.DeleteEntry(10);
        }

        [TestMethod]
        public void UpdateEntry_ExistsAlready_ShouldSuceed()
        {
            var repository = new StoredPasswordRepository(_password);

            repository.AddEntry(new StoredPassword(_correctDomain, _password));

            var newPassword = new SecureString();
            newPassword.PopulateWithString("TESTING");
            var newDto = new StoredPassword(_correctDomain, newPassword);

            repository.UpdateEntry(newDto);

            var updatedPassword = repository.GetPasswordFromDomain(_correctDomain);

            Assert.AreEqual(updatedPassword.ConvertToUnsecureString(), newPassword.ConvertToUnsecureString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateEntry_EntryDoesNotExist_ShouldFail()
        {
            var repository = new StoredPasswordRepository(_password);

            repository.AddEntry(new StoredPassword(_correctDomain, _password));

            var newDto = new StoredPassword("UGGA BUGGA", _password);

            repository.UpdateEntry(newDto);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetPasswordFromDomain_WrongDomain_ShouldReturnNull()
        {
            var repository = new StoredPasswordRepository(_password);

            repository.AddEntry(new StoredPassword(_correctDomain, _password));

            var password = repository.GetPasswordFromDomain(_correctDomain + "not");
        }

    }
}
