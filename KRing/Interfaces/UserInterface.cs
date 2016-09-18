using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using KRing;
using KRing.Core;
using KRing.Core.Model;
using KRing.DTO;
using KRing.Persistence.Model;

namespace KRing.Interfaces
{
    public interface IUserInterface
    {
        void StartupMessage();

        void WelcomeMessage(User User);

        void MessageToUser(string msg);

        void ShowAllDomainsToUser(IEnumerable<DBEntry> entries);

        void GoodbyeMessage(User User);

        SecureString RequestPassword(string msg);

        DbEntryDto RequestNewEntryInformation(User user);

        string RequestUserInput(string msg);

        string RequestUsername();

        void BadLogin();

        void LoginTimeoutMessage();

        bool YesNoQuestionToUser(string question);

        ActionType MainMenu();

    }
}
