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
        /* swe should get both username and passsword before checking both, and just reporting error, not what is wrong */
        static void Main(string[] args)
        {
            UserInterface UI = new ConsoleLineInterface();

            UI.StartupMessage();

            string username = UI.RequestUserName();

            string password = UI.RequestPassword();

            Session currentSession = Authenticator.LogIn(username, password);

            if (currentSession.User.IsLoggedIn)
            {
                UI.WelcomeMessage(currentSession.User);
            }
            else
            {
                UI.BadLogin();
            }


        }
    }
}
