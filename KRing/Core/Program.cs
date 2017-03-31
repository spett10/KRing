using System;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using KRing.Core.Controllers;
using KRing.Core.Model;
using KRing.Core.View;
using KRing.Extensions;
using KRing.Interfaces;
using KRing.Persistence.Controllers;
using KRing.Persistence.Repositories;
using System.Collections.Generic;

namespace KRing.Core
{
    class Program
    {
        private static bool _isRunning;
        private static Session _currentSession;
        private static DbController _dbController;
        private static IUserInterface _ui;
        private static IPasswordUI _pswdUi;
        private static ProfileController _profileController;
        private static bool _doesProfileExist = false;

        private static List<Action> _actions = new List<Action> {
                                                ProgramInit,
                                                NewOrReturningUser,
                                                LoginLoop,
                                                TryToLoadDB,
                                                ProgramLoop };

        static void Main(string[] args)
        {
            foreach(var programStep in _actions)
            {
                try
                {
                    programStep();
                }
                catch(Exception)
                {
                    return;
                }
            }

            return;            
        }

        /// <summary>
        /// Setup the UI and prepares state before login attempt
        /// </summary>
        private static void ProgramInit()
        {
            _ui = new ConsoleLineInterface();
            _pswdUi = new ConsoleLineInterface();
            _ui.StartupMessage();
            _isRunning = false;
            _currentSession = Session.DummySession();
        }

        /// <summary>
        /// If no profile is stored, we handle the new user. Else, we load the profile of the existing user
        /// </summary>
        private static void NewOrReturningUser()
        {
            _doesProfileExist = true;
            _profileController = new ProfileController();

            try
            {
                _profileController.LoadProfile();
            }
            catch (Exception)
            {
                _doesProfileExist = false;
            }

            if (!_doesProfileExist)
            {
                HandleNewUser();
            }
        }

        /// <summary>
        /// If noone fiddled with the underlying encrypted files, this will suceed. If someone fiddled with them, we fail, since we cant decrypt.
        /// If no profile is found, we purge the DB. We do that on a profile delete too, but we need to be sure that nothing is saved. 
        /// </summary>
        private static void TryToLoadDB()
        {
            _dbController = new DbController(new DbEntryRepository(_currentSession.User.Password));
            if (_currentSession.IsLoggedIn)
            {
                _isRunning = true;
                if (_doesProfileExist)
                {
                    try
                    {
                        _dbController.LoadDb();
                    }
                    catch (Exception)
                    {
                        _ui.MessageToUser("Internal Error, config files corrupted.");
                    }
                }
                else
                {
                    _dbController.DeleteAllEntries();
                }
            }
        }

        /// <summary>
        /// Presents the main menu to the user, and handles request by dispatching to the appropriate event handlers. 
        /// </summary>
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

        /// <summary>
        /// Dispatches to the profile controller, checks the password etc.
        /// </summary>
        private static void LoginLoop()
        {
            _currentSession = _profileController.LoginLoop(_ui);
        }

        /// <summary>
        /// Purge everything at the request of the user.
        /// </summary>
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
            _profileController.NewProfile(_ui, _pswdUi);
        }

        private static void HandleDeletePassword()
        {
            _dbController.DeletePassword(_ui);
        }


        private static void HandleAddPassword()
        {
            _dbController.AddPassword(_ui, _currentSession.User);
        }

        private static void HandleLogout()
        {
            _isRunning = false;
            _currentSession.IsLoggedIn = false;
            _ui.GoodbyeMessage(_currentSession.User);

            try
            {
                _dbController.SaveAllEntries();
            }
            catch(Exception)
            {
                //No entries to save.. 
            }
        }

        private static void HandleUpdatePassword()
        {
            _dbController.UpdatePassword(_ui, _pswdUi);
        }

        private static void HandleViewPassword()
        {
            _dbController.ViewPassword(_ui);

        }

    }


}
