using System;
using KRing.Persistence.Interfaces;
using KRing.Persistence.Model;
using KRing.Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Windows.Forms;
using static KRingForm.PasswordList;

namespace KRingForm.Forms
{
    public partial class EditPasswordForm : Form
    {
        private readonly IDbEntryRepository _passwordRep;
        private readonly UpdateListCallback _callback;
        private string _editTarget;

        public EditPasswordForm(IDbEntryRepository repository, UpdateListCallback callback, string domain)
        {
            InitializeComponent();
            _passwordRep = repository;
            _callback = callback;
            _editTarget = domain;

            domainBox.Text = domain;
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            var plaintextPassword = passwordBox.Text;

            if(plaintextPassword == String.Empty)
            {
                MessageBox.Show("Please enter new password", "Error");
            }
            else
            {
                var password = new SecureString();
                password.PopulateWithString(plaintextPassword);

                var newEntry = new DBEntry(_editTarget, password);
                
                _passwordRep.UpdateEntry(newEntry);

                _callback();

                this.Close();
            }
        }
    }
}
