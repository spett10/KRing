using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using KRingCore.Core;
using KRingCore.Core.Model;
using KRingCore.Persistence.Model;

namespace KRing.Interfaces
{
    public interface IUserInterface
    {
        void StartupMessage();

        void WelcomeMessage(User User);

        void MessageToUser(string msg);

        void ShowAllDomainsToUser(IEnumerable<StoredPassword> entries);

        void GoodbyeMessage(User User);

        SecureString RequestPassword(string msg);

        StoredPassword RequestNewEntryInformation(User user);

        string RequestUserInput(string msg);

        string RequestUsername();

        void BadLogin();

        void LoginTimeoutMessage();

        bool YesNoQuestionToUser(string question);

        ActionType MainMenu();

    }
}
