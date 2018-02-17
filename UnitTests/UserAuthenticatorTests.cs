using KRingCore.Core.Services;
using KRingCore.Security;
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

            var usernameSalt = CryptoHashing.GenerateSalt();
            var usernameRaw = Encoding.ASCII.GetBytes(username);
            var userToken = UserAuthenticator.CreateAuthenticationToken(username, usernameSalt);

            var passwordSalt = CryptoHashing.GenerateSalt();
            var passwordRaw = Encoding.ASCII.GetBytes(password);
            var passwordToken = UserAuthenticator.CreateAuthenticationToken(password, passwordSalt);

            var passwordCorrect = UserAuthenticator.Authenticate(passwordRaw, passwordSalt, passwordToken);

            var usernameCorrect = UserAuthenticator.Authenticate(usernameRaw, usernameSalt, userToken);

            Assert.IsTrue(passwordCorrect);
            Assert.IsTrue(usernameCorrect);
        }

        [TestMethod]
        public void UserAuthentication_WrongPassword_ShouldBeDifferent()
        {
            var username = "Sugga pie";
            var password = "so secret!";

            var usernameSalt = CryptoHashing.GenerateSalt();
            var usernameRaw = Encoding.ASCII.GetBytes(username);
            var userToken = UserAuthenticator.CreateAuthenticationToken(username, usernameSalt);

            var passwordSalt = CryptoHashing.GenerateSalt();
            var passwordRaw = Encoding.ASCII.GetBytes("Some guess");
            var passwordToken = UserAuthenticator.CreateAuthenticationToken(password, passwordSalt);

            var passwordCorrect = UserAuthenticator.Authenticate(passwordRaw, passwordSalt, passwordToken);

            var usernameCorrect = UserAuthenticator.Authenticate(usernameRaw, usernameSalt, userToken);

            Assert.IsFalse(passwordCorrect);
            Assert.IsTrue(usernameCorrect);
        }
    }
}
