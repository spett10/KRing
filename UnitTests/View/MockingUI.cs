using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using KRing.Core;
using KRing.Core.Model;
using KRing.Interfaces;
using KRing.Persistence.Model;
using KRing.Extensions;

namespace UnitTests
{
    class MockingUI : IUserInterface, IPasswordUI
    {
        public string _username;
        private readonly SecureString _password;
        public int IndexToAnswer { get; set; }

        public bool answerWithRandomPassword = false;

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

        public DBEntry RequestNewEntryInformation(User user)
        {
            if(answerWithRandomPassword)
            {
                var password = new SecureString();

                //Make random password... 
                //They should, with good prop. at least, be random enough that the password advisor that might check them, should call them good.
                password.PopulateWithString(Convert.ToBase64String(CryptoHashing.GenerateSalt())); 
                return new DBEntry(_username, password);
            }
            else
            {
                return new DBEntry(_username, _password);
            }
        }

        public SecureString RequestPassword(string msg)
        {
            return _password;
        }

        public string RequestUserInput(string msg)
        {
            return IndexToAnswer.ToString();
        }

        public void ShowAllDomainsToUser(IEnumerable<DBEntry> entries)
        {
            return;
        }

        public void StartupMessage()
        {
            throw new NotImplementedException();
        }

        public void WelcomeMessage(User User)
        {
            return;
        }

        public bool YesNoQuestionToUser(string question)
        {
            throw new NotImplementedException();
        }
    }
}
