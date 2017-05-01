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
        private readonly IStoredPasswordRepository _passwordRep;
        private readonly UpdateListCallback _callback;
        private PasswordGenerator _generator;

        private string _editTarget;

        private bool _generateClicked = false;

        public EditPasswordForm(IStoredPasswordRepository repository, UpdateListCallback callback, string domain)
        {
            InitializeComponent();
            _passwordRep = repository;
            _callback = callback;
            _editTarget = domain;

            _generator = new PasswordGenerator();

            domainBox.Text = domain;
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            var plaintextPassword = passwordBox.Text;

            if(plaintextPassword == String.Empty)
            {
                Program._messageToUser("Please enter new password or generate it");
            }
            else
            {
                var newEntry = new StoredPassword(_editTarget, plaintextPassword);
                
                _passwordRep.UpdateEntry(newEntry);

                _callback(OperationType.EditPassword);

                this.Close();
            }
        }

        private void largeSizeButton_CheckedChanged(object sender, EventArgs e)
        {
            _generator.Size = PasswordGenerator.PasswordSize.Large;
        }

        private void mediumSizeButton_CheckedChanged(object sender, EventArgs e)
        {
            _generator.Size = PasswordGenerator.PasswordSize.Medium;
        }

        private void smallSizeButton_CheckedChanged(object sender, EventArgs e)
        {
            _generator.Size = PasswordGenerator.PasswordSize.Small;
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            if (!_generateClicked)
            {
                passwordBox.Text = _generator.Generate();

                /* Disable buttons since you can only generate once */
                smallSizeButton.Enabled = false;
                mediumSizeButton.Enabled = false;
                largeSizeButton.Enabled = false;
                generateButton.Enabled = false;

                _generateClicked = true;
            }
        }
    }
}
