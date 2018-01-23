using System;
using KRingCore.Persistence.Interfaces;
using KRingCore.Persistence.Model;
using KRingCore.Extensions;
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

        public EditPasswordForm(IStoredPasswordRepository repository, UpdateListCallback callback, StoredPassword entry)
        {
            InitializeComponent();
            _passwordRep = repository;
            _callback = callback;
            _editTarget = entry.Domain;

            _generator = new PasswordGenerator();
            
            domainBox.Text = entry.Domain;
            usernameBox.Text = entry.Username;
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            Notify();

            var plaintextPassword = passwordBox.Text.ToCharArray();

            if(plaintextPassword.Length < 1)
            {
                Program._messageToUser("Please enter new password or generate it");
            }
            else
            {
                var newEntry = new StoredPassword(_editTarget, usernameBox.Text, plaintextPassword);
                
                _passwordRep.UpdateEntry(newEntry);

                _callback(OperationType.EditPassword);

                plaintextPassword.ZeroOut();

                this.Close();
            }
        }

        private void largeSizeButton_CheckedChanged(object sender, EventArgs e)
        {
            Notify();
            _generator.Size = PasswordGenerator.PasswordSize.Large;
        }

        private void mediumSizeButton_CheckedChanged(object sender, EventArgs e)
        {
            Notify();
            _generator.Size = PasswordGenerator.PasswordSize.Medium;
        }

        private void smallSizeButton_CheckedChanged(object sender, EventArgs e)
        {
            Notify();
            _generator.Size = PasswordGenerator.PasswordSize.Small;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Notify();
            _generator.Size = PasswordGenerator.PasswordSize.Largest;
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            Notify();

            if (!_generateClicked)
            {
                passwordBox.Text = _generator.Generate();

                /* Disable buttons since you can only generate once */
                smallSizeButton.Enabled = false;
                mediumSizeButton.Enabled = false;
                largeSizeButton.Enabled = false;
                largestSizeButton.Enabled = false;
                generateButton.Enabled = false;

                _generateClicked = true;
            }
        }

        private void Notify()
        {
            ActivityManager.Instance.Notify();
        }

        
    }
}
