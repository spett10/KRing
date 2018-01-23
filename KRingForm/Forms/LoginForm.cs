using System;
using System.Windows.Forms;
using KRingCore.Core.Model;
using KRingCore.Security;
using static KRingForm.Program;
using KRingCore.Core.Services;
using System.Threading.Tasks;
using KRingForm.Forms;

namespace KRingForm
{
    public partial class LoginForm : Form
    {
        private User savedUser;
        private readonly LoginCallback _callback;

        private const int maxLoginAttemps = 3;
        private int usedLoginAttempts = 0;

        public LoginForm(User user, LoginCallback callback)
        {
            savedUser = user;
            _callback = callback;
            InitializeComponent();
        }

        //TODO: do work here async while showing some load stuff.
        private async void loginButton_Click(object sender, EventArgs e)
        {
            var loadingForm = new Loading();
            

            var userName = usernameBox.Text;
            var password = passwordBox.Text;

            

            if(userName == string.Empty || password == string.Empty)
            {
                Program._messageToUser("Please input username and password");
            }
            else
            {
                loadingForm.Show();

                var authenticateTask = Task<bool>.Run(() =>
                {
                    var correctUsername = UserAuthenticator.Authenticate(userName, savedUser.SecurityData.UsernameHashSalt, savedUser.SecurityData.HashedUsername);

                    var correctPassword = UserAuthenticator.Authenticate(password, savedUser.SecurityData.PasswordHashSalt, savedUser.SecurityData.HashedPassword);

                    return correctUsername && correctPassword;
                });

                /* Check both username and password, even if username is wrong - dont leak anything timewise (enables enumeration of user) */
                var authentic = await authenticateTask;

                loadingForm.Close();

                if (!(authentic))
                {
                    HandleFailedLogon(userName);
                }
                else
                {
                    savedUser.UserName = userName;
                    savedUser.PlaintextPassword = password;
                    _callback(true);
                    this.Close();
                }
            }            
        }
        
        private void HandleFailedLogon(string user)
        {
            usedLoginAttempts++;
            Program._messageToUser("Wrong username and/or password");
            Program.Log("Login failed attempt", "");

            if(usedLoginAttempts >= maxLoginAttemps)
            {
                MessageBox.Show("All login attempts used.");
                Program.Log("Login failed all attempts", "");
                _callback(false);
                this.Close();
            }
        }
    }
}
