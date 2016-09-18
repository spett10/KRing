using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security;
using KRing.Extensions;
using KRing.Core;

namespace UnitTests
{
    [TestClass]
    public class PasswordAdvisorTests
    {
        [TestMethod]
        public void StrongPasswordShouldBeGood()
        {
            var strongPassword = new SecureString();

            strongPassword.PopulateWithString("VeryGoodPassword1234");

            var mockingUI = new MockingUI("TESTUSER", strongPassword);

            var result = PasswordAdvisor.CheckPasswordWithUserInteraction(mockingUI);

            Assert.AreEqual(strongPassword.ConvertToUnsecureString(), result.ConvertToUnsecureString());
        }

        [TestMethod]
        public void VeryWeakPassword()
        {
            var weakPassword = "bacon22";

            var score = PasswordAdvisor.CheckStrength(weakPassword);

            Assert.AreEqual(score, PasswordScore.VeryWeak);
        }
    }
}
