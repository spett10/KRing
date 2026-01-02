using KRingCore.Core.Model;
using KRingCore.Core.Services;
using KRingCore.Persistence.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class FlatFileLogTests
    {
        [TestMethod]
        public void AuthenticateLog_Positive()
        {
            /* Arrange */
            var log = new FlatFileErrorLog(deriveIterations: 1);
            log.ClearLog();

            log.Log("TEST", "LALALALLA");

            var userAuthenticator = new UserAuthenticator(loginIterations: 1);

            var user = User.NewUserWithFreshSalt(userAuthenticator, "JOHN DOE", "PASSWORD123");

            /* Act */
            log.AuthenticateLog(user);

            /* Assert */
            Assert.IsTrue(log.CheckLogIntegrity(user));
        }

        [TestMethod]
        public void AuthenticateLog_WrongUser()
        {
            /* Arrange */
            var log = new FlatFileErrorLog(deriveIterations: 1);
            log.ClearLog();

            log.Log("TEST", "LALALALLA");

            var userAuthenticator = new UserAuthenticator(loginIterations: 1);

            var user = User.NewUserWithFreshSalt(userAuthenticator, "JOHN DOE", "PASSWORD123");

            /* Act */
            log.AuthenticateLog(user);

            var otherUser = User.NewUserWithFreshSalt(userAuthenticator, "SOMEONE ELSE", "PASSWORD1234");

            /* Assert */
            Assert.IsFalse(log.CheckLogIntegrity(otherUser));
        }

        [TestMethod]
        public void AuthenticateLog_AlteredFile()
        {
            /* Arrange */
            var log = new FlatFileErrorLog(deriveIterations: 1);
            log.ClearLog();

            log.Log("TEST", "LALALALLA");
            
            var userAuthenticator = new UserAuthenticator(loginIterations: 1);

            var user = User.NewUserWithFreshSalt(userAuthenticator, "JOHN DOE", "PASSWORD123");

            /* Act */
            log.AuthenticateLog(user);

            /* Write some more so mac wont be correct */

            log.Log("TEST", "OOPS");

            /* Assert */
            Assert.IsFalse(log.CheckLogIntegrity(user));
        }
    }
}
