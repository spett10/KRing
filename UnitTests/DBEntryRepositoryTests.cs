using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRing.Persistence.Repositories;
using KRing.Extensions;
using System.Security;
using KRing.DTO;

namespace UnitTests
{
    [TestClass]
    public class DBEntryRepositoryTests
    {
        SecureString _password;
        string _correctDomain;

        [TestInitialize]
        public void TestInitialize()
        {
            _password = new SecureString();
            _password.PopulateWithString("YELLOW SUBMARINE");

            _correctDomain = "test";
        }

        [TestMethod]
        public void CreateDBRepository()
        {
            var repository = new DbEntryRepository(_password);

            Assert.AreEqual(repository.EntryCount, 0);
        }

        [TestMethod]
        public void AddEntry_ExistsEntry_ShouldExist()
        {
            var repository = new DbEntryRepository(_password);

            repository.AddEntry(new DbEntryDto(_correctDomain, _password));

            var exists = repository.ExistsEntry(_correctDomain);

            Assert.AreEqual(exists, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Error: Domain Already Exists")]
        public void AddEntry_AddAgain_ShouldThrowException()
        {
            var repository = new DbEntryRepository(_password);

            repository.AddEntry(new DbEntryDto(_correctDomain, _password));

            //duplicates are not allowed
            repository.AddEntry(new DbEntryDto(_correctDomain, _password));
        }

        [TestMethod]
        public void AddEntryThenDeleteIt_ExistsEntry_ShouldNotExist()
        {
            var repository = new DbEntryRepository(_password);

            repository.AddEntry(new DbEntryDto(_correctDomain, _password));
            repository.DeleteEntry(_correctDomain);

            var exists = repository.ExistsEntry(_correctDomain);

            Assert.AreEqual(exists, false);
        }

        [TestMethod]
        public void AddSomeEntries_DeleteAll_CheckExists()
        {
            var repository = new DbEntryRepository(_password);

            string domain1 = "FooBar";
            string domain2 = "FooBaz";
            string domain3 = "Testing";

            repository.AddEntry(new DbEntryDto(domain1, _password));
            repository.AddEntry(new DbEntryDto(domain2, _password));
            repository.AddEntry(new DbEntryDto(domain3, _password));

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
            var repository = new DbEntryRepository(_password);

            string domain1 = "FooBar";
            string domain2 = "FooBaz";
            string domain3 = "Testing";

            repository.AddEntry(new DbEntryDto(domain1, _password));
            repository.AddEntry(new DbEntryDto(domain2, _password));
            repository.AddEntry(new DbEntryDto(domain3, _password));

            repository.WriteEntriesToDb();

            //load db into new object, with same password. 
            var otherRepository = new DbEntryRepository(_password);
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

    }
}
