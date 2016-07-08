using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            while (!LoggedIn && attempts <= MaxLoginAttempts)
            {
                string username = UI.RequestUserName();

                string password = UI.RequestPassword();

                Session currentSession = Authenticator.LogIn(username, password);

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
                        return;
                    }
                    
                }
            }
            
        }
    }
}
