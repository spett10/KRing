using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRing
{
    class Program
    {
        /* swe should get both username and passsword before checking both, and just reporting error, not what is wrong */
        static void Main(string[] args)
        {
            UserInterface.StartupMessage();

            string username = UserInterface.RequestUserName();

            string password = UserInterface.RequestPassword();

            Session currentSession = Authenticator.LogIn(username, password);

            if (currentSession.User.IsLoggedIn)
            {
                UserInterface.WelcomeMessage(currentSession.User);
            }
            else
            {
                UserInterface.BadLogin();
            }


        }
    }
}
