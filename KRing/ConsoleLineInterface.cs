using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using KRing.Interfaces;
using KRing.DTO;

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

        public void MessageToUser(string msg)
        {
            Console.WriteLine(msg);
        }

        public void WelcomeMessage(User User)
        {
            if(User.IsLoggedIn) Console.WriteLine("\nWelcome {0}\n", User.UserName);
        }

        public void GoodbyeMessage(User User)
        {
            if (User.UserName != "Guest") Console.WriteLine("Goodbye {0}", User.UserName);
            else Console.WriteLine("Goodbye");

        }

        public SecureString RequestPassword(string msg)
        {
            Console.WriteLine(msg);
            SecureString password = new SecureString();

            ConsoleKeyInfo nextKey = Console.ReadKey(true);

            while(nextKey.Key != ConsoleKey.Enter)
            {
                if(nextKey.Key == ConsoleKey.Backspace)
                {
                    if(password.Length > 0)
                    {
                        password.RemoveAt(password.Length - 1);

                        Console.Write(nextKey.KeyChar);
                        Console.Write(" ");
                        Console.Write(nextKey.KeyChar);
                    }
                }
                else
                {
                    password.AppendChar(nextKey.KeyChar);
                    Console.Write("*");
                }

                nextKey = Console.ReadKey(true);
            }

            return password;
        }

        public DBEntryDTO RequestNewEntryInformation(User user)
        {
            string domain = RequestUserInput("Please enter domain associated with password you want to store");
            SecureString password = RequestPassword("Please Enter the password to be stored");

            return new DBEntryDTO(domain, password);
        }

        public string RequestUserInput(string msg)
        {
            Console.WriteLine(msg);

            string EnteredUserInput = Console.ReadLine();

            return EnteredUserInput;
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
                        NewCommand = ActionType.ViewPassword;
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
