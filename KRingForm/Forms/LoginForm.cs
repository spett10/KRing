using System;
using System.Windows.Forms;
using KRingCore.Core.Model;
using KRingCore.Security;
using static KRingForm.Program;

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

        private void loginButton_Click(object sender, EventArgs e)
        {
            var userName = usernameBox.Text;
            var password = passwordBox.Text;

            if(userName == string.Empty || password == string.Empty)
            {
                Program._messageToUser("Please input username and password");
            }
            else
            {
                /* Check both username and password, even if username is wrong - dont leak anything timewise (enables enumeration of user) */
                var correctUsername = CryptoHashing.CompareSaltedHash(userName, savedUser.Cookie.UsernameHashSalt, savedUser.Cookie.HashedUsername);

                var correctPassword = CryptoHashing.CompareSaltedHash(password, savedUser.Cookie.PasswordHashSalt, savedUser.Cookie.HashedPassword);

                if (!(correctPassword && correctUsername))
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
            Program.Log("Login failed attempt", "User: " + user);

            if(usedLoginAttempts >= maxLoginAttemps)
            {
                MessageBox.Show("All login attempts used.");
                Program.Log("Login failed all attempts", "User: " + user);
                _callback(false);
                this.Close();
            }
        }
    }
}
