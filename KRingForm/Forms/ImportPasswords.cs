using KRingCore.Core.Services;
using KRingCore.Persistence.Interfaces;
using KRingCore.Security;
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
        private readonly ImportCallback _callback;
        private readonly ErrorCallback _errorCallback;
        private readonly UpdateListCallback _revealingCallback;
        private readonly Form _hidingBehind;


        public ImportPasswords(OpenFileDialog dialogue, ImportCallback importCallback, ErrorCallback errorCallback, IStreamReadToEnd streamReader, UpdateListCallback revealingCallback, Form hidingBehind)
        {
            InitializeComponent();

            _dialogue = dialogue;
            _streamReader = streamReader;
            _callback = importCallback;
            _errorCallback = errorCallback;
            _revealingCallback = revealingCallback;
            _hidingBehind = hidingBehind;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Enabled = false;

            Notify();

            if (string.IsNullOrEmpty(this.passwordBox.Text))
            {
                this.passwordEmptyLabel.Visible = true;
                this.Enabled = true;
            }
            else
            {
                var password = new SecureString();
                foreach(var c in this.passwordBox.Text)
                {
                    password.AppendChar(c);
                }

                Import(password);

                this.Enabled = true;
                this.Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Notify();
            this.Close();
        }

        private void Notify()
        {
            ActivityManager.Instance.Notify();
        }

        private void Import(SecureString password)
        {
            var importer = new DecryptingPasswordImporter(new KRingCore.Security.KeyGenerator(), password);

            try
            {
                var importedPasswords = importer.ImportPasswords(_dialogue.FileName, _streamReader);

                _callback(importedPasswords);
            }
            catch(CryptographicException)
            {
                // Password was invalid.
                _errorCallback("Password entered for decryption was incorrect. please try again");
            }
            catch(CryptoHashing.IntegrityException)
            {
                _errorCallback("Imported file integrity compromised. Consider deleting file.");
            }
            catch(Exception)
            {
                _errorCallback("The format of the file supplied was not valid. You can only import files export by this application");
            }
        }

    }
}
