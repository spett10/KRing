using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;
using KRing.Core;
using KRing.Extensions;
using KRing.Persistence.Repositories;

namespace KRingForm
{
    public partial class LoginForm : Form
    {
        private KRing.Core.Model.User savedUser;
        private const int maxLoginAttemps = 3;
        private int usedLoginAttempts = 0;

        public LoginForm()
        {
            var reader = new ProfileRepository();
            savedUser = reader.ReadUser();

            InitializeComponent();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            var userName = usernameBox.Text;
            var password = passwordBox.Text;

            var securePassword = new SecureString();
            securePassword.PopulateWithString(password);

            var correctUsername = CheckUserName(userName);
            var correctPassword = CryptoHashing.ScryptCheckPassword(securePassword, savedUser.Cookie.HashedPassword);

            if(!(correctPassword && correctPassword))
            {
                HandleFailedLogon();
            }
            else
            {
                MessageBox.Show("Logged In");
            }


        }

        private bool CheckUserName(string username)
        {
            return username == savedUser?.UserName ? true : false;
        }

        private void HandleFailedLogon()
        {
            usedLoginAttempts++;

            if(usedLoginAttempts >= maxLoginAttemps)
            {
                MessageBox.Show("All login attempts used.");
                this.Close();
            }
        }
    }
}
