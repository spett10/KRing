using KRingCore.Persistence.Interfaces;
using KRingCore.Persistence.Model;
using KRingCore.Extensions;
using System;
using System.Windows.Forms;
using static KRingForm.PasswordList;
using KRingCore.Security;

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
            Notify();

            var domainName = domainBox.Text;
            var username = usernameBox.Text;
            var plaintextPassword = passwordBox.Text.ToCharArray();
            
            if(domainName == String.Empty || plaintextPassword.Length < 1)
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
                    var dbEntry = new StoredPassword(domainName, username, plaintextPassword);

                    _passwordRep.AddEntry(dbEntry);
                    
                    _callback(OperationType.AddPassword);

                    plaintextPassword.ZeroOut();

                    this.Close();
                }                
            }
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

        private void smallSizeButton_CheckedChanged(object sender, EventArgs e)
        {
            Notify();
            _generator.Size = PasswordGenerator.PasswordSize.Small;
        }

        private void mediumSizeButton_CheckedChanged(object sender, EventArgs e)
        {
            Notify();
            _generator.Size = PasswordGenerator.PasswordSize.Medium;
        }

        private void largeSizeButton_CheckedChanged(object sender, EventArgs e)
        {
            Notify();
            _generator.Size = PasswordGenerator.PasswordSize.Large;
        }

        private void Notify()
        {
            ActivityManager.Instance.Notify();
        }

        private void largestSizeButton_CheckedChanged(object sender, EventArgs e)
        {
            Notify();
            _generator.Size = PasswordGenerator.PasswordSize.Largest;
        }
    }

    internal class PasswordGenerator
    {
        public enum PasswordSize
        {
            Small,
            Medium, 
            Large,
            Largest
        }

        public PasswordSize Size { get; set; }

        public PasswordGenerator()
        {
            Size = PasswordSize.Largest;
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
                case PasswordSize.Largest:
                    chosenSize = 32;
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
