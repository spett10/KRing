using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRingCore.Core;

namespace UnitTests
{
    /// <summary>
    /// Summary description for ConfigurationTests
    /// </summary>
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void Configuration_LoadingCorrectValues()
        {
            var loginIterations = Configuration.PBKDF2LoginIterations;
            var loginIterationsOld = Configuration.OLD_PBKDF2LoginIterations;
            var deriveIterations = Configuration.PBKDF2DeriveIterations;
            var deriveIterationsOld = Configuration.OLD_PBKDF2DeriveIterations;

            Assert.IsTrue(loginIterations > 0);
            Assert.IsTrue(loginIterationsOld > 0);
            Assert.IsTrue(deriveIterations > 0);
            Assert.IsTrue(deriveIterationsOld > 0);
        }
    }
}
