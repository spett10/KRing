using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using KRing.Interfaces;
using KRing.DTO;
using KRing.DB;

namespace KRing
{
    class Program
    {
        static void Main(string[] args)
        {
            IUserInterface UI = new ConsoleLineInterface();
            
            UI.StartupMessage();

            int MaxLoginAttempts = 3;
            int attempts = 0;
            bool LoggedIn = false;

            Session currentSession = new Session(new User("Guest", false, new SecureString()));

            /* Login Loop */
            while (!LoggedIn)
            {
                string username = UI.RequestUserInput("Please Enter Username:");

                SecureString password = UI.RequestPassword("Please Enter Your Password");
                currentSession = Authenticator.LogIn(username, password);

                if (currentSession.User.IsLoggedIn)
                {
                    UI.WelcomeMessage(currentSession.User);
                    LoggedIn = true;
                }
                else
                {
                    UI.BadLogin();
                    attempts++;

                    if (attempts >= MaxLoginAttempts)
                    {
                        UI.LoginTimeoutMessage();
                        password.Dispose();
                        return;
                    }   
                }
                password.Clear();
            }

            /* User Logged In */
            DBController db = DBController.Instance;
            bool IsRunning = true;
            
            while (IsRunning)
            {
                ActionType nextAction = UI.MainMenu();

                switch (nextAction)
                {
                    case ActionType.Logout:
                        IsRunning = false;
                        currentSession.User.Logout();
                        UI.GoodbyeMessage(currentSession.User);
                        db.Write(currentSession.User.Password.ConvertToUnsecureString());
                        break;

                    case ActionType.AddPassword:
                        DBEntryDTO newEntry = UI.RequestNewEntryInformation(currentSession.User);
                        db.AddEntry(newEntry);
                        break;
                }

            }

            /* Program Exit */
            return;
            
        }
    }
}
