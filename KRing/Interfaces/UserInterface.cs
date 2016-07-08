using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using KRing;

namespace KRing.Interfaces
{
    public interface IUserInterface
    {
        void StartupMessage();

        void WelcomeMessage(User User);

        void GoodbyeMessage(User User);

        SecureString RequestPassword();

        string RequestUserName();

        void BadLogin();

        void LoginTimeoutMessage();

        ActionType MainMenu();

    }
}
