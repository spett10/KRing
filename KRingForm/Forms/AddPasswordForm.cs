using KRing.Persistence.Interfaces;
using KRing.Persistence.Model;
using KRing.Extensions;
using KRing.Persistence.Repositories;
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
using KRing.Core;

namespace KRingForm.Forms
{
    public partial class AddPasswordForm : Form
    {
        private readonly IStoredPasswordRepository _passwordRep;
        private readonly UpdateListCallback _callback;
        private readonly PasswordGenerator _generator;

        private bool _generateClicked = false;

        public AddPasswordForm(IStoredPasswordRepository repository, UpdateListCallback callback)
        {
            InitializeComponent();
            _passwordRep = repository;
            _callback = callback;

            _generator = new PasswordGenerator();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            var domainName = domainBox.Text;
            var plaintextPassword = passwordBox.Text;
            
            if(domainName == String.Empty || plaintextPassword == String.Empty)
            {
                Program._messageToUser("Please enter Domain, and either enter or generate a password.");
            }
            else
            {
                if(_passwordRep.ExistsEntry(domainName))
                {
                    Program._messageToUser("Domain already exists in stored passwords");
                }
                else
                {
                    var dbEntry = new StoredPassword(domainName, plaintextPassword);

                    _passwordRep.AddEntry(dbEntry);
                    
                    _callback(OperationType.AddPassword);

                    this.Close();
                }                
            }
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

        private void smallSizeButton_CheckedChanged(object sender, EventArgs e)
        {
            _generator.Size = PasswordGenerator.PasswordSize.Small;
        }

        private void mediumSizeButton_CheckedChanged(object sender, EventArgs e)
        {
            _generator.Size = PasswordGenerator.PasswordSize.Medium;
        }

        private void largeSizeButton_CheckedChanged(object sender, EventArgs e)
        {
            _generator.Size = PasswordGenerator.PasswordSize.Large;
        }
    }

    internal class PasswordGenerator
    {
        public enum PasswordSize
        {
            Small,
            Medium, 
            Large
        }

        public PasswordSize Size { get; set; }

        public PasswordGenerator()
        {
            Size = PasswordSize.Large;
        }

        public string Generate()
        {
            var chosenSize = 16;
            switch(Size)
            {
                case PasswordSize.Small:
                    chosenSize = 8;
                    break;
                case PasswordSize.Medium:
                    chosenSize = 12;
                    break;
                case PasswordSize.Large:
                    chosenSize = 16;
                    break;
            }

            /* We want to display/save the random bytes in base64, but we dont want padding. So we choose a byte size */
            /* That requires no padding, and will produce as many characters as the user requested. Since base64 takes 6 bits
             * for each character, and a byte is 8 bits, we get the below fraction (reduced, of course) */

            var saltSize = (chosenSize / 4) * 3;
            return Convert.ToBase64String(CryptoHashing.GenerateSalt(saltSize));
        }
    }
}
