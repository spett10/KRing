using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KRing.Extensions;
using KRing.Core.Model;
using KRing.Persistence.Repositories;

namespace KRingForm
{
    public partial class CreateUserForm : Form
    {
        public CreateUserForm()
        {
            InitializeComponent();
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            var username = usernameBox.Text;

            //Todo: Password strength or requirements?
            var password = passwordBox.Text;
            var securePassword = new SecureString();
            securePassword.PopulateWithString(password);

            var profileRep = new ProfileRepository();

            var user = User.NewUserWithFreshSalt(username, securePassword);

            /* Update Profile */
            profileRep.WriteUser(user);

            /* Update Config */
            var config = new ConfigRepository();
            config.UserExists();            
        }
    }
}
