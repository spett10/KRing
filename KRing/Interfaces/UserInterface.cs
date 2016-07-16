using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using KRing;
using KRing.DTO;

namespace KRing.Interfaces
{
    public interface IUserInterface
    {
        void StartupMessage();

        void WelcomeMessage(User User);

        void MessageToUser(string msg);

        void GoodbyeMessage(User User);

        SecureString RequestPassword(string msg);

        DBEntryDTO RequestNewEntryInformation(User user);

        string RequestUserInput(string msg);

        void BadLogin();

        void LoginTimeoutMessage();

        ActionType MainMenu();

    }
}
