using System;
using System.Security;
using KRing.Core.Model;
using KRing.Extensions;
using KRing.Interfaces;
using KRing.Persistence.Repositories;
using KRing.Persistence.Interfaces;
using System.Diagnostics;

namespace KRing.Core.Controllers
{
    public class ProfileController
    {
        public int UserCount { get; private set; }
        private readonly IProfileRepository _profileRepository;
        private User _user;
        private readonly int _maxLoginAttempts = 3;
        private int _usedLoginAttempts = 0;

        public ProfileController()
        {
            UserCount = 1;
            _profileRepository = new ProfileRepository();
        }

        public ProfileController(IProfileRepository _rep)
        {
            UserCount = 1;
            _profileRepository = _rep;
        }

        public void NewProfile(IUserInterface ui, IPasswordUI pswdUI)
        {
            ui.MessageToUser("Creating new user for you!");
            var newUserName = ui.RequestUsername();

            SecureString password = TryGetStrongPassword(pswdUI);
            
            var newUser = User.NewUserWithFreshSalt(newUserName, password);

            UpdateProfile(newUser);
        }

        private SecureString TryGetStrongPassword(IPasswordUI ui)
        {
            bool consistentPasswordInput = false;

            SecureString password = new SecureString();

            while (!consistentPasswordInput)
            {
                password = PasswordAdvisor.CheckPasswordWithUserInteraction(ui);

                var passwordRepeated = ui.RequestPassword("\nPlease re-enter your desired password");

                consistentPasswordInput =
                    password.ConvertToUnsecureString()
                        .Equals(passwordRepeated.ConvertToUnsecureString(), StringComparison.OrdinalIgnoreCase);

                if (!consistentPasswordInput) ui.MessageToUser("\nPasswords were not consistent. Please try again");
            }

            return password;
        }

        public Session LoginLoop(IUserInterface ui)
        {
            ui.MessageToUser("\nPlease Log In");

            bool isLoggedIn = false;
            Session newSession = Session.DummySession();

            while (!isLoggedIn)
            {
                string username = ui.RequestUsername();

                SecureString password = ui.RequestPassword("Please Enter Your Password");

                try
                {
                    newSession = LogIn(username, password);
                }
                catch (Exception e)
                {
                    ui.MessageToUser(e.Message);
                    _usedLoginAttempts++;

                    if (_usedLoginAttempts >= _maxLoginAttempts)
                    {
                        ui.LoginTimeoutMessage();
                        password.Dispose();
                        throw new UnauthorizedAccessException("All login attempts used");
                    }
                }


                if (newSession.IsLoggedIn)
                {
                    ui.WelcomeMessage(newSession.User);
                    isLoggedIn = true;
                }
                password.Clear();
            }

            return newSession;
        }

        private Session LogIn(string username, SecureString password)
        {
            var storedSaltedPassword = _user.Cookie.HashedPassword;

            //were do we get the password? Cos we load the wrong things actually.. 
            bool isPasswordCorrect = CryptoHashing.ScryptCheckPassword(password, _user.Cookie.HashSalt, storedSaltedPassword);

            bool isCorrectUser = username.Equals(_user.UserName, StringComparison.InvariantCultureIgnoreCase);

            bool allowLogin = isPasswordCorrect && isCorrectUser;

            if (allowLogin)
            {
                _user.Password = password;
                return new Session(_user, true);
            }
            else
            {
                throw new UnauthorizedAccessException("Username and/or password was incorrect");
            }
        }

        private void UpdateProfile(User user)
        {
            if(user == null) throw new ArgumentNullException();

                _profileRepository.WriteUser(user);

                _user = _profileRepository.ReadUser();
        }

        public void DeleteProfile()
        {
            _profileRepository.DeleteUser();
        }

        /// <summary>
        /// The reason we dont call this in the constructor, is that it can fail. I like it more when a constructor cant fail,
        /// even though the class is not actually initialized/usable before we load the user.
        /// So even though we would like to "initialization is resource allocation" it up, ive chosen this approach instead.
        /// </summary>
        public void LoadProfile()
        {
            try
            {
                _user = _profileRepository.ReadUser();
            }
            catch(Exception)
            {
                throw;
            }
        }

        

    }
}
