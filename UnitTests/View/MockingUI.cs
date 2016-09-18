using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using KRing.Core;
using KRing.Core.Model;
using KRing.DTO;
using KRing.Interfaces;
using KRing.Persistence.Model;

namespace UnitTests
{
    class MockingUI : IUserInterface
    {
        private readonly string _username;
        private readonly SecureString _password;

        public MockingUI()
        {
            _username = "test";
            _password = new SecureString();
            _password.AppendChar('A');
        }

        public MockingUI(string username, SecureString password)
        {
            _username = username;
            _password = password;
        }

        public void BadLogin()
        {
            throw new NotImplementedException();
        }

        public void GoodbyeMessage(User User)
        {
            throw new NotImplementedException();
        }

        public void LoginTimeoutMessage()
        {
            return;
        }

        public string RequestUsername()
        {
            return _username;
        }

        public ActionType MainMenu()
        {
            throw new NotImplementedException();
        }

        public void MessageToUser(string msg)
        {
            return;
        }

        public DbEntryDto RequestNewEntryInformation(User user)
        {
            throw new NotImplementedException();
        }

        public SecureString RequestPassword(string msg)
        {
            return _password;
        }

        public string RequestUserInput(string msg)
        {
            throw new NotImplementedException();
        }

        public void ShowAllDomainsToUser(IEnumerable<DBEntry> entries)
        {
            throw new NotImplementedException();
        }

        public void StartupMessage()
        {
            throw new NotImplementedException();
        }

        public void WelcomeMessage(User User)
        {
            throw new NotImplementedException();
        }

        public bool YesNoQuestionToUser(string question)
        {
            throw new NotImplementedException();
        }
    }
}
