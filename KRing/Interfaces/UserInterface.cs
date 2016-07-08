using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRing;

namespace KRing.Interfaces
{
    public interface IUserInterface
    {
        void StartupMessage();

        void WelcomeMessage(User User);

        void GoodbyeMessage(User User);

        string RequestPassword();

        string RequestUserName();

        void BadLogin();

        void LoginTimeoutMessage();

        ActionType MainMenu();

    }
}
