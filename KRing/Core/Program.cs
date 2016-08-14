using System;
using System.Linq;
using System.Security;
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
                                    new Cookie(Authenticator.GenerateSalt(),
                                                Authenticator.GenerateSalt(),
                                                Authenticator.GenerateSalt())));

            /* Login Loop */
            while (!_isLoggedIn)
            {
                string username = _ui.RequestUserInput("Please Enter Username:");

                SecureString password = _ui.RequestPassword("Please Enter Your Password");

                try
                {
                    _currentSession = Authenticator.LogIn(username, password);
                }
                catch (Exception e)
                {
                    _ui.MessageToUser(e.Message);
                    _usedLoginAttempts++;

                    if (_usedLoginAttempts >= _maxLoginAttempts)
                    {
                        _ui.LoginTimeoutMessage();
                        password.Dispose();
                        return;
                    }
                }
                

                if (_currentSession.User.IsLoggedIn)
                {
                    _ui.WelcomeMessage(_currentSession.User);
                    _isLoggedIn = true;
                }
                password.Clear();
            }

            /* User Logged In */
            _dbController = DBController.Instance;
            _isRunning = true;
            
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
                }

            }

            /* Program Exit */
            return;
            
        }

        private static void HandleDeletePassword()
        {
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
