using System;
using System.Security;
using KRing.Core.Model;
using KRing.Persistence;
using KRing.Persistence.Repositories;

namespace KRing.Core.Controllers
{
    public class ProfileController
    {
        public int UserCount { get; private set; }
        private readonly ProfileRepository _profileRepository;
        private User _user;

        public ProfileController()
        {
            UserCount = 1;
            _profileRepository = new ProfileRepository();
        }

        public Session LogIn(string username, SecureString password)
        {
            bool isPasswordCorrect = false;

            var storedSaltedPassword = _user.Cookie.PasswordSalted;
            var givenPasswordWhenSalted = CryptoHashing.GenerateSaltedHash(password, _user.Cookie.SaltForPassword);

            isPasswordCorrect = CryptoHashing.CompareByteArrays(storedSaltedPassword, givenPasswordWhenSalted);

            bool isCorrectUser = username.Equals(_user.UserName, StringComparison.Ordinal);

            bool allowLogin = isPasswordCorrect && isCorrectUser;

            if (allowLogin)
            {
                _user.Login();
                _user.Password = password;
                return new Session(_user);
            }
            else
            {
                throw new UnauthorizedAccessException("Username and/or password was incorrect");
            }
        }

        public void NewProfile(User user)
        {
            if(user == null) Console.WriteLine("User not good");

            _profileRepository.WriteUser(user);

            _user = _profileRepository.ReadUser();
        }

        public void DeleteProfile()
        {
            _profileRepository.DeleteUser();
        }

        public void LoadProfile()
        {
            _user = _profileRepository.ReadUser();
        }

        

    }
}
