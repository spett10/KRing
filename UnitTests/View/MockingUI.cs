﻿using System;
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
        private readonly string _plaintextPassword = "A";
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

        public StoredPassword RequestNewEntryInformation(User user)
        {
            if(answerWithRandomPassword)
            {
                //Make random password... 
                //They should, with good prop. at least, be random enough that the password advisor that might check them, should call them good.
                return new StoredPassword(_username, Convert.ToBase64String(CryptoHashing.GenerateSalt()));
            }
            else
            {
                return new StoredPassword(_username, _plaintextPassword);
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

        public void ShowAllDomainsToUser(IEnumerable<StoredPassword> entries)
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
