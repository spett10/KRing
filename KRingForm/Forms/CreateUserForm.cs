using System;
using System.Collections.Generic;
using System.Security;
using System.Windows.Forms;
using KRingCore.Extensions;
using KRingCore.Core.Model;
using KRingCore.Persistence.Repositories;
using KRingCore.Core.Interfaces;
using KRingCore.Core;
using System.Threading.Tasks;

namespace KRingForm
{
    public partial class CreateUserForm : Form
    {        
        public CreateUserForm()
        {
            InitializeComponent();
        }

        private async void createButton_Click(object sender, EventArgs e)
        {
            this.Enabled = false;

            var username = usernameBox.Text;

            var password = passwordBox.Text;

            if(password == null || password == String.Empty)
            {
                Program._messageToUser("Password cannot be empty");
                return;
            }

            var score = PasswordAdvisor.CheckStrength(password);

            if (IsPasswordStrongEnough(score))
            {
                var profileRep = new ProfileRepository();
                
                var user = await Task<User>.Run(() => { return User.NewUserWithFreshSalt(username, password); });

                

                var writeUserTask = profileRep.WriteUserAsync(user);

                /* Callback to set User */
                Program.SavedUser = user;
                Program.userCreated = true;

                /* Update Profile */
                await writeUserTask;

                this.Enabled = true;

                this.Close();
            }
            else
            {
                this.Enabled = true;
                Program._messageToUser("Password not strong enough - it must have at least one special character, one capital character and one digit! Or, it has to have a length of at least 16.");
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
