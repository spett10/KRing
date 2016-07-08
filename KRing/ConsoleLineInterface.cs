using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRing.Interfaces;

namespace KRing
{
    /* Todo: flyt al text over i en shallow static class? */
    public class ConsoleLineInterface : IUserInterface
    {
        public ConsoleLineInterface()
        {

        }

        public void StartupMessage()
        {
            Console.WriteLine("Welcome To KRing");
        }

        public void WelcomeMessage(User User)
        {
            if(User.IsLoggedIn) Console.WriteLine("Welcome {0}", User.UserName);
        }

        public void GoodbyeMessage(User User)
        {
            if (User.UserName != "Guest") Console.WriteLine("Goodbye {0}", User.UserName);
            else Console.WriteLine("Goodbye");
        }

        public string RequestPassword()
        {
            Console.WriteLine("Please enter your password:");
            string password = null;
            while(true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                password += key.KeyChar;
            }

            return password;
        }

        public string RequestUserName()
        {
            Console.WriteLine("Please Enter Username:");

            string EnteredUsername = Console.ReadLine();

            return EnteredUsername;
        }

        public void BadLogin()
        {
            Console.WriteLine("Username and/or password was incorrect");
        }

        public void LoginTimeoutMessage()
        {
            Console.WriteLine("\nToo many wrong attempts. Exiting.");
        }


        /* Todo: Remove the magic strings for the commands. I.e define a variable that is the given consolekey, and somehow extract the key
         * and make it a string and use that in the UI? */
        public ActionType MainMenu()
        {
            Console.WriteLine("\nPlease enter an action:\n");
            Console.WriteLine("V: View Password");
            Console.WriteLine("A: Add Password");
            Console.WriteLine("U: Update Password");
            Console.WriteLine("D: Delete Password");
            Console.WriteLine("L: Logout");
            Console.WriteLine("\n");

            bool AwaitingCommand = true;
            ActionType NewCommand = ActionType.AddPassword; //Dummy.

            while (AwaitingCommand)
            {
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.V:
                        NewCommand = ActionType.ViewPasswords;
                        AwaitingCommand = false;
                        break;
                    case ConsoleKey.A:
                        NewCommand = ActionType.AddPassword;
                        AwaitingCommand = false;
                        break;
                    case ConsoleKey.U:
                        NewCommand = ActionType.UpdatePassword;
                        AwaitingCommand = false;
                        break;
                    case ConsoleKey.D:
                        NewCommand = ActionType.DeletePassword;
                        AwaitingCommand = false;
                        break;
                    case ConsoleKey.L:
                        NewCommand = ActionType.Logout;
                        AwaitingCommand = false;
                        break;
                    default:
                        Console.WriteLine("Command Not Recognized. Try Again");
                        break;
                }
            }

            return NewCommand;
        }

    }
}
