using System;
using System.Collections.Generic;
using System.Security;
using System.Windows.Forms;
using KRingCore.Extensions;
using KRingCore.Core.Model;
using KRingCore.Persistence.Repositories;
using KRingCore.Core.Interfaces;
using KRingCore.Core;

namespace KRingForm
{
    public partial class CreateUserForm : Form
    {
        private List<IPasswordRule> _rules;
        
        public CreateUserForm()
        {
            InitializeComponent();
        }

        private async void createButton_Click(object sender, EventArgs e)
        {
            var username = usernameBox.Text;

            //Todo: Password strength or requirements?
            var password = passwordBox.Text;

            if(password == null || password == String.Empty)
            {
                Program._messageToUser("Password cannot be empty");
                return;
            }

            var score = PasswordAdvisor.CheckStrength(password);

            if (IsPasswordStrongEnough(score))
            {
                var securePassword = new SecureString();
                securePassword.PopulateWithString(password);

                var profileRep = new ProfileRepository();

                var user = User.NewUserWithFreshSalt(username, securePassword);

                var writeUserTask = profileRep.WriteUserAsync(user);

                /* Callback to set User */
                Program.SavedUser = user;
                Program.userCreated = true;

                /* Update Profile */
                await writeUserTask;

                this.Close();
            }
            else
            {
                Program._messageToUser("Password not strong enough - it must have at least one special character, one capital character and one digit!");
            }            
        }

        private bool IsPasswordStrongEnough(PasswordScore score)
        {
            bool passwordStrongEnough = false;

            switch (score)
            {
                case PasswordScore.Blank:
                case PasswordScore.VeryWeak:
                case PasswordScore.Weak:
                    passwordStrongEnough = false;
                    break;
                case PasswordScore.Medium:
                case PasswordScore.Strong:
                case PasswordScore.VeryStrong:
                    passwordStrongEnough = true;
                    break;
            }

            return passwordStrongEnough;
        }

    }
}
