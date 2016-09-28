using System;
using KRing.Persistence.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security;
using KRing.Extensions;
using KRing.Core.Model;
using KRing.Core;
using UnitTests.Repositories;

namespace UnitTests
{
    [TestClass]
    public class DBControllerTests
    {
        private SecureString _password;
        private string _username;
        private Cookie _cookie;

        [TestInitialize]
        public void TestInitialize()
        {
            _password = new SecureString();
            _password.PopulateWithString("TESTING");

            _username = "test";

            _cookie = new Cookie(CryptoHashing.GenerateSalt(), CryptoHashing.GenerateSalt(), CryptoHashing.GenerateSalt());
        }

        [TestMethod]
        public void AddPasword_ShouldSuceed()
        {
            var repository = new MockingDBEntryRepository();

            var controller = new DbController(repository);

            var user = new User(_username, _password, _cookie);

            var ui = new MockingUI(_username, _password);

            controller.AddPassword(ui, user);

            Assert.AreEqual(repository._addEntryCalled, true);
        }
        
        [TestMethod]
        public void UpdatePassword_GiveCorrectIndex_ShouldSucceed()
        {
            var repository = new MockingDBEntryRepository();

            var controller = new DbController(repository);

            var user = new User(_username, _password, _cookie);

            var pwd = new SecureString(); pwd.PopulateWithString("TESTING");

            var ui = new MockingUI(_username, pwd);

            controller.AddPassword(ui, user);

            ui.IndexToAnswer = repository.EntryCount;

            controller.UpdatePassword(ui);

            Assert.AreEqual(repository._updateEntryCalled, true);
        }

        [TestMethod]
        public void DeletePassword_GiveCorrectDomain_ShouldCallRepository()
        {
            var repository = new MockingDBEntryRepository();

            var controller = new DbController(repository);

            var user = new User(_username, _password, _cookie);

            var ui = new MockingUI(_username, _password);

            controller.AddPassword(ui, user);

            ui.IndexToAnswer = repository.EntryCount;

            controller.DeletePassword(ui);

            Assert.AreEqual(repository._deleteEntryCalled, true);
        }

        [TestMethod]
        public void DeletePassword_NoPasswordsStored_ShouldNotCallRepository()
        {
            var repository = new MockingDBEntryRepository();

            var controller = new DbController(repository);

            var ui = new MockingUI(_username, _password);

            controller.DeletePassword(ui);

            Assert.AreEqual(repository._deleteEntryCalled, false);
        }

        [TestMethod]
        public void ViewPassword_GiveCorrectDomain_ShouldCallRepository()
        {
            var repository = new MockingDBEntryRepository();

            var controller = new DbController(repository);

            var ui = new MockingUI(_username, _password);
            
            var user = new User(_username, _password, _cookie);

            controller.AddPassword(ui, user);

            ui.IndexToAnswer = repository.EntryCount;

            controller.ViewPassword(ui);

            Assert.AreEqual(repository._viewEntryCalled, true);
        }

        [TestMethod]
        public void ViewPassword_NoPasswordsStored_ShouldNotCallRepository()
        {
            var repository = new MockingDBEntryRepository();

            var controller = new DbController(repository);

            var ui = new MockingUI();

            controller.ViewPassword(ui);

            Assert.AreEqual(repository._viewEntryCalled, false);
        }

        [TestMethod]
        public void SaveEntries_ThenLoad_ShouldSucceed()
        {
            var repository = new MockingDBEntryRepository();

            var controller = new DbController(repository);

            var ui = new MockingUI(_username, _password);

            var user = new User(_username, _password, _cookie);

            controller.AddPassword(ui, user);
            ui.IndexToAnswer = repository.EntryCount;

            controller.SaveAllEntries();

            Assert.AreEqual(repository._saveCalled, true);

            controller.LoadDb();
            Assert.AreEqual(controller.EntryCount, 1);
            Assert.AreEqual(repository._loadCalled, true);

            controller.DeletePassword(ui);
            Assert.AreEqual(controller.EntryCount, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SaveEmptyEntries_ShouldThrowError()
        {
            var repository = new MockingDBEntryRepository();

            var controller = new DbController(repository);

            controller.SaveAllEntries();
        }

        [TestMethod]
        public void DeleteAllEntries_ShouldCallRepository_ShouldHaveZeroCount()
        {
            var repository = new MockingDBEntryRepository();

            var controller = new DbController(repository);

            var ui = new MockingUI(_username, _password);
            ui.answerWithRandomPassword = true;

            var user = new User(_username, _password, _cookie);
            
            controller.AddPassword(ui, user);
            controller.AddPassword(ui, user);
            controller.AddPassword(ui, user);
            controller.AddPassword(ui, user);

            Assert.AreEqual(controller.EntryCount, 4);

            controller.DeleteAllEntries();

            Assert.AreEqual(repository._deleteAllCalled, true);
            Assert.AreEqual(controller.EntryCount, 0);
        }
    }
}
