﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRingCore.Persistence.Logging;
using KRingCore.Core.Model;
using System.Configuration;

namespace UnitTests
{
    [TestClass]
    public class FlatFileLogTests
    {
        private readonly string _logfilePath = ConfigurationManager.AppSettings["relativeLogPathDebug"];
        private readonly string _logIntegrityFile = ConfigurationManager.AppSettings["relativeLogIntegrityPathDebug"];

        [TestMethod]
        public void AuthenticateLog_Positive()
        {
            /* Arrange */
            var log = new FlatFileErrorLog();
            log.ClearLog();

            log.Log("TEST", "LALALALLA");

            var user = User.NewUserWithFreshSalt("JOHN DOE", "PASSWORD123");

            /* Act */
            log.AuthenticateLog(user);

            /* Assert */
            Assert.IsTrue(log.CheckLogIntegrity(user));
        }

        [TestMethod]
        public void AuthenticateLog_WrongUser()
        {
            /* Arrange */
            var log = new FlatFileErrorLog();
            log.ClearLog();

            log.Log("TEST", "LALALALLA");
            
            var user = User.NewUserWithFreshSalt("JOHN DOE", "PASSWORD123");

            /* Act */
            log.AuthenticateLog(user);

            var otherUser = User.NewUserWithFreshSalt("SOMEONE ELSE", "PASSWORD1234");

            /* Assert */
            Assert.IsFalse(log.CheckLogIntegrity(otherUser));
        }

        [TestMethod]
        public void AuthenticateLog_AlteredFile()
        {
            /* Arrange */
            var log = new FlatFileErrorLog();
            log.ClearLog();

            log.Log("TEST", "LALALALLA");

            var user = User.NewUserWithFreshSalt("JOHN DOE", "PASSWORD123");

            /* Act */
            log.AuthenticateLog(user);

            /* Write some more so mac wont be correct */

            log.Log("TEST", "OOPS");

            /* Assert */
            Assert.IsFalse(log.CheckLogIntegrity(user));
        }
    }
}
