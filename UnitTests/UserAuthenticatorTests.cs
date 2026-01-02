using KRingCore.Core.Services;
using KRingCore.Krypto;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace UnitTests
{
    [TestClass]
    public class UserAuthenticatorTests
    {
        [TestMethod]
        public void UserAuthentication_SuccessCase_ShouldBeEqual()
        {
            var username = "Sugga pie";
            var password = "so secret!";

            var userAuthenticator = new UserAuthenticator(loginIterations: 1);

            var usernameSalt = CryptoHashing.GenerateSalt();
            var usernameRaw = Encoding.ASCII.GetBytes(username);
            var userToken = userAuthenticator.CreateAuthenticationToken(username, usernameSalt);

            var passwordSalt = CryptoHashing.GenerateSalt();
            var passwordRaw = Encoding.ASCII.GetBytes(password);
            var passwordToken = userAuthenticator.CreateAuthenticationToken(password, passwordSalt);

            var passwordCorrect = userAuthenticator.Authenticate(passwordRaw, passwordSalt, passwordToken);

            var usernameCorrect = userAuthenticator.Authenticate(usernameRaw, usernameSalt, userToken);

            Assert.IsTrue(passwordCorrect);
            Assert.IsTrue(usernameCorrect);
        }

        [TestMethod]
        public void UserAuthentication_WrongPassword_ShouldBeDifferent()
        {
            var username = "Sugga pie";
            var password = "so secret!";

            var userAuthenticator = new UserAuthenticator(loginIterations: 1);

            var usernameSalt = CryptoHashing.GenerateSalt();
            var usernameRaw = Encoding.ASCII.GetBytes(username);
            var userToken = userAuthenticator.CreateAuthenticationToken(username, usernameSalt);

            var passwordSalt = CryptoHashing.GenerateSalt();
            var passwordRaw = Encoding.ASCII.GetBytes("Some guess");
            var passwordToken = userAuthenticator.CreateAuthenticationToken(password, passwordSalt);

            var passwordCorrect = userAuthenticator.Authenticate(passwordRaw, passwordSalt, passwordToken);

            var usernameCorrect = userAuthenticator.Authenticate(usernameRaw, usernameSalt, userToken);

            Assert.IsFalse(passwordCorrect);
            Assert.IsTrue(usernameCorrect);
        }
    }
}
