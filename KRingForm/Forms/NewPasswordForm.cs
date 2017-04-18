using KRing.Core;
using KRing.Persistence.Interfaces;
using KRing.Persistence.Model;
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
using static KRingForm.PasswordList;

namespace KRingForm.Forms
{
    public partial class NewPasswordForm : Form
    {
        private readonly IStoredPasswordRepository _passwordRep;
        private readonly UpdateListCallback _callback;

        private bool _generateClicked = false;
        private int chosenSize;

        public NewPasswordForm(IStoredPasswordRepository repository, UpdateListCallback callback)
        {
            InitializeComponent();
            _passwordRep = repository;
            _callback = callback;

            chosenSize = 16;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if(domainBox.Text == null || domainBox.Text == String.Empty)
            {
                Program._messageToUser("Please enter domain");
                return;
            }

            if (!_generateClicked)
            {
                Program._messageToUser("Please generate random password before you add it to storage");
                return;
            }

            var password = passwordBox.Text;

            var dbEntry = new StoredPassword(domainBox.Text, password);

            _passwordRep.AddEntry(dbEntry);

            _callback(OperationType.NewPassword);

            this.Close();
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            if (!_generateClicked)
            {
                /* We want to display/save the random bytes in base64, but we dont want padding. So we choose a byte size */
                /* That requires no padding, and will produce as many characters as the user requested. Since base64 takes 6 bits
                 * for each character, and a byte is 8 bits, we get the below fraction (reduced, of course) */
                var saltSize = (chosenSize / 4) * 3;

                passwordBox.Text = Convert.ToBase64String(CryptoHashing.GenerateSalt(saltSize));

                /* Disable buttons since you can only generate once */
                smallSizeButton.Enabled = false;
                mediumSizeButton.Enabled = false;
                largeSizeButton.Enabled = false;
                generateButton.Enabled = false;

                _generateClicked = true;
            }            
        }

        private void smallSizeButton_CheckedChanged(object sender, EventArgs e)
        {
            chosenSize = 8;
        }

        private void mediumSizeButton_CheckedChanged(object sender, EventArgs e)
        {
            chosenSize = 12;
        }

        private void largeSizeButton_CheckedChanged(object sender, EventArgs e)
        {
            chosenSize = 16;
        }
    }
}
