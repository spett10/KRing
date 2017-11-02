using KRingCore.Core.Model;
using KRingCore.Persistence.Logging;
using KRingCore.Persistence.Repositories;
using System;
using System.Windows.Forms;
using KRingForm.Forms;

namespace KRingForm
{
    public delegate void MessageToUser(string message);
    public delegate void Log(string context, string message);

    public static class Program
    {
        public delegate void LoginCallback(bool isLoginSuccessful);

        public static bool isLoggedIn;
        public static User SavedUser;

        public static readonly MessageToUser _messageToUser = s => { MessageBox.Show(s); };
        public static readonly Log Log = (c, s) => { _log.Log(c, s); };

        private static FlatFileErrorLog _log = new FlatFileErrorLog();
        public static bool userCreated;

        public static bool userInactiveLogout = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {

                isLoggedIn = false;
                userCreated = false;

                /* Check if a user exists; If not, create new user, if it does, show logon format */
                if (!DoesUserExist())
                {
                    try
                    {
                        Application.Run(new CreateUserForm());
                    }
                    catch (Exception e)
                    {
                        _messageToUser("Error: " + e.Message);
                        Log("Main", e.Message);
                    }
                }

                /* Try to loging */
                if (DoesUserExist() || userCreated)
                {
                    try
                    {
                        Application.Run(new LoginForm(SavedUser, loginCallback));
                    }
                    catch (Exception e)
                    {
                        _messageToUser("Error: " + e.Message);
                        Log("Main", e.Message);
                    }
                }


                /* Run the passwordlist if successful login */
                if (isLoggedIn)
                {
                    try
                    {
                        Application.Run(new PasswordList(SavedUser));
                    }
                    catch (Exception e)
                    {   
                        //TODO: show something to user, i.e. fatal exception occured. 
                        Log("Main", "PasswordList got exception");
                    }
                }

                /* If passwordlist was closed and the user was inactive, show dialogue */
                if (userInactiveLogout)
                {
                    Application.Run(new InactiveDialogue());
                    Log("Main", "User timed out");
                }

            }
            catch
            {
                //Supress any unforseen errors. 
            }
            finally
            {
                //TODO: try to force garbage collection? Call some handle that tries to wipe memory. Anything. 
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
