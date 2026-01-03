using KRingCore.Core.Model;
using KRingCore.Core.Services;
using KRingCore.Persistence.Repositories;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            if (string.IsNullOrEmpty(password))
            {
                Program._messageToUser("Password cannot be empty");
                return;
            }



            var profileRep = new ProfileRepository();

            var userAuthenticator = new UserAuthenticator(Configuration.Configuration.PBKDF2LoginIterations,
                                                          Configuration.Configuration.OLD_PBKDF2LoginIterations,
                                                          Configuration.Configuration.TryOldValues);

            var user = await Task<User>.Run(() => { return User.NewUserWithFreshSalt(userAuthenticator, username, password); });

            var writeUserTask = profileRep.WriteUserAsync(user);

            /* Callback to set User */
            Program.SavedUser = user;
            Program.userCreated = true;

            /* Update Profile */
            await writeUserTask;

            this.Enabled = true;

            this.Close();

        }
    }
}
