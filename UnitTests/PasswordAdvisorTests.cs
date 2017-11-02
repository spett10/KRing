using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRingCore.Core;

namespace UnitTests
{
    [TestClass]
    public class PasswordAdvisorTests
    {

        [TestMethod]
        public void VeryWeakPassword()
        {
            var weakPassword = "bacon22";

            var score = PasswordAdvisor.CheckStrength(weakPassword);

            Assert.AreEqual(score, PasswordScore.VeryWeak);
        }
    }
}
