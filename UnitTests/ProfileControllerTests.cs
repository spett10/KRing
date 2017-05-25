using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRing.Core.Controllers;
using System.Security;
using KRingCore.Extensions;

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

            profileCtrl.LoadProfile();
            var session = profileCtrl.LoginLoop(_givesCorrectPassword);

            Assert.IsTrue(session.IsLoggedIn);
        }

        [TestMethod]
        public void CreateNewUserStrongPassword()
        {
            var profileCtrl = new ProfileController(new MockingProfileRepository(correctUsername, strongPasswordSecureString));

            profileCtrl.NewProfile(_givesCorrectPassword, _givesCorrectPassword);

            //add loging stuff too to test 

            var session = profileCtrl.LoginLoop(_givesCorrectPassword);

            Assert.IsTrue(session.IsLoggedIn);
        }
    }
}
