using System;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using KRing.DB;
using KRing.DTO;
using KRing.Extensions;
using KRing.Interfaces;

namespace KRing.Core
{
    class Program
    {
        private static int _maxLoginAttempts;
        private static int _usedLoginAttempts;
        private static bool _isLoggedIn;
        private static bool _isRunning;
        private static Session _currentSession;
        private static DBController _dbController;
        private static IUserInterface _ui;
        private static ProfileController _profileController;

        static void Main(string[] args)
        {
            /* Setup */
            _ui = new ConsoleLineInterface();
            _ui.StartupMessage();

            _maxLoginAttempts = 3;
            _usedLoginAttempts = 0;
            _isLoggedIn = false;
            _isRunning = false;
            
            /* ew, fix this TODO */
            _currentSession = new Session(new User("Guest", false, new SecureString(),
                                    new Cookie(CryptoHashing.GenerateSalt(),
                                                CryptoHashing.GenerateSalt(),
                                                CryptoHashing.GenerateSalt())));

            /* new user or returning user? */
            bool doesProfileExist = true;
            _dbController = DBController.Instance;
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
                if(doesProfileExist) _dbController.LoadDb();
            }

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

            /* Program Exit */
            return;
            
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
                _dbController.DeleteDb();
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

            var saltForPassword = CryptoHashing.GenerateSalt();
            var saltedPassword = CryptoHashing.GenerateSaltedHash(password.ConvertToUnsecureString(), saltForPassword);
            var saltForKey = CryptoHashing.GenerateSalt();
            
            var cookie = new Cookie(saltedPassword, saltForPassword, saltForKey);
            var newUser = new User(newUserName, false, password, cookie);

            _profileController.NewProfile(newUser);
            _dbController.DeleteDb();
        }

        private static void HandleDeletePassword()
        {
            if (_dbController.EntryCount <= 0)
            {
                _ui.MessageToUser("You have no passwords stored\n");
                return;
            }

            ShowAllDomainsToUser();

            var correctDomainGiven = false;
            var domain = string.Empty;

            while (!correctDomainGiven)
            {
                domain = _ui.RequestUserInput("Please Enter Domain to Delete");
                correctDomainGiven = _dbController.ExistsEntry(domain);

                if(!correctDomainGiven) _ui.MessageToUser("That domain does not exist amongst stored passwords");
            }

            _dbController.DeleteEntryFromDomain(domain);
        }


        private static void HandleAddPassword()
        {
            DBEntryDTO newEntry = _ui.RequestNewEntryInformation(_currentSession.User);
            try
            {
                _dbController.AddEntry(newEntry);
            }
            catch (ArgumentException e)
            {
                _ui.MessageToUser("\n" + e.Message);
            }
        }

        private static void HandleLogout()
        {
            _isRunning = false;
            _currentSession.User.Logout();
            _ui.GoodbyeMessage(_currentSession.User);
            _dbController.WriteDb(_currentSession.User.Password.ConvertToUnsecureString());
        }

        private static void HandleUpdatePassword()
        {
            if (_dbController.EntryCount <= 0)
            {
                _ui.MessageToUser("You have no passwords stored\n");
                return;
            }

            ShowAllDomainsToUser();

            var correctDomainGiven = false;
            var domain = String.Empty;

            while (!correctDomainGiven)
            {
                domain = _ui.RequestUserInput("Please Enter Domain to Update");
                correctDomainGiven = _dbController.ExistsEntry(domain);

                if (!correctDomainGiven) _ui.MessageToUser("That Domain Does not exist amongst stored passwords");
            }

            var newPassword = _ui.RequestPassword("Please enter new password for the domain " + domain);
            _dbController.UpdateEntry(new DBEntryDTO(domain, newPassword));
        }

        private static void ShowAllDomainsToUser()
        {
            _ui.MessageToUser("Stored Domains:");

            foreach (var entr in _dbController.Entries)
            {
                Console.WriteLine(entr.Domain);
            }
        }

        private static void HandleViewPassword()
        {
            if (_dbController.EntryCount <= 0)
            {
                _ui.MessageToUser("You have no passwords stored\n");
                return;
            }

            bool correctDomainGiven = false;
            string domain = String.Empty;

            ShowAllDomainsToUser();

            while(!correctDomainGiven)
            {
                domain = _ui.RequestUserInput("Please Enter Domain to get corresponding Password");
                correctDomainGiven = _dbController.ExistsEntry(domain);

                if (!correctDomainGiven) _ui.MessageToUser("That Domain Does not exist amongst stored passwords");
            }
            
            var entry = _dbController.GetPassword(domain);

            _ui.MessageToUser("Password for domain " + domain + " is:\n\n " + entry.ConvertToUnsecureString());

        }

    }


}
