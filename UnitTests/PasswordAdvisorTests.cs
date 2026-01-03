using KRingCore.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            Assert.AreEqual(PasswordScore.VeryWeak, score);
        }
    }
}
