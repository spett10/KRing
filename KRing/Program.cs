using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using KRing.Interfaces;

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

            Session currentSession = new Session(new User("Guest", false));

            /* Login Loop */
            while (!LoggedIn)
            {
                string username = UI.RequestUserName();

                SecureString password = UI.RequestPassword();
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
                        break;

                    case ActionType.AddPassword:

                        break;
                }

            }

            /* Program Exit */
            return;
            
        }
    }
}
