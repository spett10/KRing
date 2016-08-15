using System;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using KRing.Core.Controllers;
using KRing.Core.Model;
using KRing.Core.View;
using KRing.DTO;
using KRing.Extensions;
using KRing.Interfaces;
using KRing.Persistence.Controllers;

namespace KRing.Core
{
    class Program
    {
        private static int _maxLoginAttempts;
        private static int _usedLoginAttempts;
        private static bool _isLoggedIn;
        private static bool _isRunning;
        private static Session _currentSession;
        private static DbController _dbController;
        private static IUserInterface _ui;
        private static ProfileController _profileController;

        static void Main(string[] args)
        {
            /* Setup */
            ProgramInit();

            /* new user or returning user? */
            bool doesProfileExist = true;
            _dbController = DbController.Instance;
            _profileController = new ProfileController();

            try
            {
                _profileController.LoadProfile();
            }
            catch (Exception)
            {
                doesProfileExist = false;
            }


            /* Login Loop */
            if (!doesProfileExist)
            {
                HandleNewUser();
            }

            try
            {
                LoginLoop();
            }
            catch (Exception)
            {
                return;
            }

            /* User Logged In */
            if (_isLoggedIn)
            {
                _isRunning = true;
                if (doesProfileExist)
                {
                    try
                    {
                        _dbController.LoadDb();
                    }
                    catch (Exception)
                    {
                        _ui.MessageToUser("");
                    }
                }
            }

            ProgramLoop();

            /* Program Exit */
            return;
            
        }

        private static void ProgramInit()
        {
            _ui = new ConsoleLineInterface();
            _ui.StartupMessage();

            _maxLoginAttempts = 3;
            _usedLoginAttempts = 0;
            _isLoggedIn = false;
            _isRunning = false;
            _currentSession = Session.DummySession();
        }

        private static void ProgramLoop()
        {
            while (_isRunning)
            {
                ActionType nextAction = _ui.MainMenu();

                switch (nextAction)
                {
                    case ActionType.DeletePassword:
                        HandleDeletePassword();
                        break;

                    case ActionType.ViewPassword:
                        HandleViewPassword();
                        break;

                    case ActionType.UpdatePassword:
                        HandleUpdatePassword();
                        break;

                    case ActionType.Logout:
                        HandleLogout();
                        break;

                    case ActionType.AddPassword:
                        HandleAddPassword();
                        break;

                    case ActionType.DeleteUser:
                        HandleDeleteUser();
                        break;
                }
            }
        }

        private static void LoginLoop()
        {
            _ui.MessageToUser("\nPlease Log In");

            while (!_isLoggedIn)
            {
                string username = _ui.RequestUserInput("\nPlease Enter Username:");

                SecureString password = _ui.RequestPassword("Please Enter Your Password");

                try
                {
                    _currentSession = _profileController.LogIn(username, password);
                }
                catch (Exception e)
                {
                    _ui.MessageToUser(e.Message);
                    _usedLoginAttempts++;

                    if (_usedLoginAttempts >= _maxLoginAttempts)
                    {
                        _ui.LoginTimeoutMessage();
                        password.Dispose();
                        throw new UnauthorizedAccessException("All login attempts used");
                    }
                }


                if (_currentSession.User.IsLoggedIn)
                {
                    _ui.WelcomeMessage(_currentSession.User);
                    _isLoggedIn = true;
                }
                password.Clear();
            }
        }

        private static void HandleDeleteUser()
        {
            bool areYouSure = _ui.YesNoQuestionToUser("Are you sure you want to delete user and all stored information? (Y/N)");

            if (areYouSure)
            {
                _profileController.DeleteProfile();
                _dbController.DeleteAllEntries();
                _isRunning = false;
                _ui.MessageToUser("Everything deleted. Goodbye.");
                Thread.Sleep(2000);
            }
        }

        private static void HandleNewUser()
        {
            _ui.MessageToUser("Creating new user for you!");
            var newUserName = _ui.RequestUserInput("Please enter your username");

            bool consistentPasswordInput = false;
            
            SecureString password = new SecureString();

            while (!consistentPasswordInput)
            {
                password = _ui.RequestPassword("\nPlease enter your desired password");
                var passwordRepeated = _ui.RequestPassword("\nPlease re-enter your desired password");

                consistentPasswordInput =
                    password.ConvertToUnsecureString()
                        .Equals(passwordRepeated.ConvertToUnsecureString(), StringComparison.OrdinalIgnoreCase);

                if(!consistentPasswordInput) _ui.MessageToUser("\nPasswords were not consistent. Please try again");
            }

            /* move this into user as constructor ? */
            var saltForPassword = CryptoHashing.GenerateSalt();
            var saltedPassword = CryptoHashing.GenerateSaltedHash(password.ConvertToUnsecureString(), saltForPassword);
            var saltForKey = CryptoHashing.GenerateSalt();
            
            var cookie = new Cookie(saltedPassword, saltForPassword, saltForKey);
            var newUser = new User(newUserName, false, password, cookie);

            _profileController.NewProfile(newUser);
            _dbController.DeleteAllEntries();
        }

        private static void HandleDeletePassword()
        {
            _dbController.DeletePassword(_ui);
        }


        private static void HandleAddPassword()
        {
            _dbController.AddPassword(_ui, _currentSession);
        }

        private static void HandleLogout()
        {
            _isRunning = false;
            _currentSession.User.Logout();
            _ui.GoodbyeMessage(_currentSession.User);
            _dbController.SaveAllEntries();
        }

        private static void HandleUpdatePassword()
        {
            _dbController.UpdatePassword(_ui);
        }

        private static void HandleViewPassword()
        {
            _dbController.ViewPassword(_ui);

        }

    }


}
