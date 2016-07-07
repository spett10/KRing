using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRing.Interfaces;

namespace KRing
{
    public class ConsoleLineInterface : UserInterface
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
    }
}
