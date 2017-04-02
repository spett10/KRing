using KRing.Core.Controllers;
using KRing.Core.Model;
using KRing.Interfaces;
using KRing.Persistence;
using KRing.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KRingForm
{
    public static class Program
    {
        public delegate void LoginCallback(bool isLoginSuccessful);

        public static bool isLoggedIn;
        public static User SavedUser;
        public static IDataConfig _config;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

             _config = new DataConfig(ConfigurationManager.AppSettings["metaPathDebug"],
                                     ConfigurationManager.AppSettings["dbPathDebug"],
                                     ConfigurationManager.AppSettings["configPathDebug"]);

            isLoggedIn = false;

            /* Check if a user exists; If not, create new user, if it does, show logon format */
            if (!DoesUserExist())
            {
                Application.Run(new CreateUserForm());
            }

            Application.Run(new LoginForm(SavedUser, loginCallback));

            if(isLoggedIn)
            {
                Application.Run(new PasswordList(SavedUser));

            }

        }

        public static void loginCallback(bool isLoginSuccessful)
        {
            isLoggedIn = isLoginSuccessful;
        }

        static bool DoesUserExist()
        {
            var profileRepository = new ProfileRepository();

            try
            {
                SavedUser = profileRepository.ReadUser();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
