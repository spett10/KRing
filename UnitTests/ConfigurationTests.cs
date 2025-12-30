using KRingCore.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            Assert.IsGreaterThan(0, loginIterations);
            Assert.IsGreaterThan(0, loginIterationsOld);
            Assert.IsGreaterThan(0, deriveIterations);
            Assert.IsGreaterThan(0, deriveIterationsOld);
        }
    }
}
