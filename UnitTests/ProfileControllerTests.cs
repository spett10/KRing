using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRing.Core.Controllers;
using KRing.Persistence.Interfaces;
using KRing.Persistence.Repositories;
using KRing.Interfaces;
using System.Security;
using System.Diagnostics;
using KRing.Extensions;
using KRing.Core;

namespace UnitTests
{
    [TestClass]
    public class ProfileControllerTests
    {
        /* make two UIs, one that returns correct one that returns incorrect password. */
        MockingUI _givesIncorrectPassword;
        MockingUI _givesCorrectPassword;

        string strongPasswordString = "123VeryStrong118";
        SecureString strongPasswordSecureString = new SecureString();
        string correctUsername = "TEST";

        
        [TestInitialize]
        public void TestInitialize()
        {
            strongPasswordSecureString.PopulateWithString(strongPasswordString);
            
            _givesIncorrectPassword = new MockingUI();
            _givesCorrectPassword = new MockingUI(correctUsername, strongPasswordSecureString);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException), "All login attempts used")]
        public void BadLoginAttemptIncorrectPassword()
        {
            var profileCtrl = new ProfileController(new MockingProfileRepository());

            profileCtrl.LoginLoop(_givesIncorrectPassword);
        }

        [TestMethod]
        public void GoodLoginAttemptCorrectPassword()
        {
            var repository = new MockingProfileRepository(correctUsername, strongPasswordSecureString);
            var profileCtrl = new ProfileController(repository);

            profileCtrl.LoadProfile(); //TODO: why does the client have to call this? cant the profilecontroller do it on construction? Initalization is resource allocation and all that..
            var session = profileCtrl.LoginLoop(_givesCorrectPassword);

            Assert.AreEqual(session.IsLoggedIn, true);
        }

        [TestMethod]
        public void CreateNewUserStrongPassword()
        {
            var profileCtrl = new ProfileController(new MockingProfileRepository(correctUsername, strongPasswordSecureString));

            profileCtrl.NewProfile(_givesCorrectPassword);
        }
    }
}
