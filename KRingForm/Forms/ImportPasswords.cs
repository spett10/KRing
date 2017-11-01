using KRingCore.Core.Services;
using KRingCore.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static KRingForm.PasswordList;

namespace KRingForm.Forms
{
    public partial class ImportPasswords : Form
    {
        private readonly OpenFileDialog _dialogue;
        private readonly IStreamReadToEnd _streamReader;
        private readonly ActiveCallback _activity;
        private readonly ImportCallback _callback;
        private readonly ErrorCallback _errorCallback;

        public ImportPasswords(OpenFileDialog dialogue, ActiveCallback activity, ImportCallback importCallback, ErrorCallback errorCallback, IStreamReadToEnd streamReader)
        {
            InitializeComponent();

            _dialogue = dialogue;
            _activity = activity;
            _streamReader = streamReader;
            _callback = importCallback;
            _errorCallback = errorCallback;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            _activity();

            if(string.IsNullOrEmpty(this.passwordBox.Text))
            {
                this.passwordEmptyLabel.Visible = true;
            }
            else
            {
                var password = new SecureString();
                foreach(var c in this.passwordBox.Text)
                {
                    password.AppendChar(c);
                }

                Import(password);

                this.Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            _activity();
            this.Close();
        }

        private void Import(SecureString password)
        {
            var importer = new DecryptingPasswordImporter();

            try
            {
                var importedPasswords = importer.ImportPasswords(_dialogue.FileName, password, _streamReader);

                _callback(importedPasswords);
            }
            catch(CryptographicException)
            {
                // Password was invalid.
                _errorCallback("Password entered for decryption was incorrect. please try again");
            }
            catch(Exception)
            {
                _errorCallback("The format of the file supplied was not valid. You can only import files export by this application");
            }
        }
    }
}
