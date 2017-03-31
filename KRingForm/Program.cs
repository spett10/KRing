using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KRingForm
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            /* Check if a user exists; If not, create new user, if it does, show logon format */
            if (DoesUserExist())
            {
                Application.Run(new LoginForm());
            }
            else
            {
                Application.Run(new CreateUserForm());
            }

        }

        static bool DoesUserExist()
        {
            var config = new ConfigRepository();

            return config.DoesUserExist();
        }
    }
}
