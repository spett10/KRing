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
            _dbController = DbController.Instance(_currentSession.User.Password);
            if (_currentSession.IsLoggedIn)
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

            /* Handle User Requests */
            ProgramLoop();

            /* Program Exit */
            return;
            
        }

        private static void ProgramInit()
        {
            _ui = new ConsoleLineInterface();
            _ui.StartupMessage();
            _isRunning = false;
            _currentSession = Session.DummySession();
        }

        private static void ProgramLoop()
        {
            while (_isRunning)
            {
                var nextAction = _ui.MainMenu();

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

                    default:
                        _ui.MessageToUser("Internal Error");
                        break;
                }
            }
        }

        private static void LoginLoop()
        {
            _currentSession = _profileController.LoginLoop(_ui);
        }

        private static void HandleDeleteUser()
        {
            var areYouSure = _ui.YesNoQuestionToUser("Are you sure you want to delete user and all stored information? (Y/N)");

            if (!areYouSure) return;

            _profileController.DeleteProfile();
            _dbController.DeleteAllEntries();
            _isRunning = false;
            _ui.MessageToUser("Everything deleted. Goodbye.");
            Thread.Sleep(2000);
        }

        private static void HandleNewUser()
        {
            _profileController.NewProfile(_ui);
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
            _currentSession.IsLoggedIn = false;
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
