using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using KRing.Interfaces;
using KRing.DTO;
using KRing.DB;

namespace KRing
{
    class Program
    {
        static void Main(string[] args)
        {
            IUserInterface UI = new ConsoleLineInterface();
            
            UI.StartupMessage();

            int MaxLoginAttempts = 3;
            int attempts = 0;
            bool LoggedIn = false;

            Session currentSession = new Session(new User("Guest", false, new SecureString()));

            /* Login Loop */
            while (!LoggedIn)
            {
                string username = UI.RequestUserInput("Please Enter Username:");

                SecureString password = UI.RequestPassword("Please Enter Your Password");
                currentSession = Authenticator.LogIn(username, password);

                if (currentSession.User.IsLoggedIn)
                {
                    UI.WelcomeMessage(currentSession.User);
                    LoggedIn = true;
                }
                else
                {
                    UI.BadLogin();
                    attempts++;

                    if (attempts >= MaxLoginAttempts)
                    {
                        UI.LoginTimeoutMessage();
                        password.Dispose();
                        return;
                    }   
                }
                password.Clear();
            }

            /* User Logged In */
            DBController DB = DBController.Instance;
            bool IsRunning = true;
            
            while (IsRunning)
            {
                ActionType nextAction = UI.MainMenu();

                switch (nextAction)
                {
                    case ActionType.ViewPassword:
                        HandleViewPassword(UI, DB);
                        break;

                    case ActionType.Logout:
                        IsRunning = false;
                        currentSession.User.Logout();
                        UI.GoodbyeMessage(currentSession.User);
                        DB.Write(currentSession.User.Password.ConvertToUnsecureString());
                        break;

                        /* TODO: check for duplicate before adding */
                    case ActionType.AddPassword:
                        DBEntryDTO newEntry = UI.RequestNewEntryInformation(currentSession.User);
                        DB.AddEntry(newEntry);
                        break;
                }

            }

            /* Program Exit */
            return;
            
        }

        public static void HandleViewPassword(IUserInterface ui, DBController db)
        {
            bool correctDomainGiven = false;

            string domain = String.Empty;

            ui.MessageToUser("Stored Domains:");

            foreach(var entr in db.Entries)
            {
                Console.WriteLine(entr.Domain);
            }


            while(!correctDomainGiven)
            {
                domain = ui.RequestUserInput("Please Enter Domain to get corresponding Password");
                correctDomainGiven = db.Entries.Any(e => e.Domain.ToString().Equals(domain, StringComparison.OrdinalIgnoreCase));

                if (!correctDomainGiven) ui.MessageToUser("That Domain Does not exist amongst stored passwords");
            }

            var Entry = db.Entries.Where(e => e.Domain == domain).Select(e => e.Password).First();

            ui.MessageToUser("Password for domain " + domain + " is:\n\n " + Entry.ConvertToUnsecureString());

        }

    }


}
