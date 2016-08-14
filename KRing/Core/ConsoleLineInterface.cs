﻿using System;
using System.Security;
using KRing.DTO;
using KRing.Interfaces;

namespace KRing.Core
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
            Console.WriteLine("\n" + msg);
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
            return Console.ReadLine();
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

            bool awaitingCommand = true;
            ActionType NewCommand = ActionType.AddPassword; //Dummy.

            while (awaitingCommand)
            {
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.V:
                        NewCommand = ActionType.ViewPassword;
                        awaitingCommand = false;
                        break;
                    case ConsoleKey.A:
                        NewCommand = ActionType.AddPassword;
                        awaitingCommand = false;
                        break;
                    case ConsoleKey.U:
                        NewCommand = ActionType.UpdatePassword;
                        awaitingCommand = false;
                        break;
                    case ConsoleKey.D:
                        NewCommand = ActionType.DeletePassword;
                        awaitingCommand = false;
                        break;
                    case ConsoleKey.L:
                        NewCommand = ActionType.Logout;
                        awaitingCommand = false;
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
