using KRingCore.Core.Services;
using KRingCore.Persistence.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using static KRingForm.PasswordList;

namespace KRingForm.Forms
{
    public enum RequestPasswordAction
    {
        Ok = 0,
        Cancel = 1
    }

    //TODO: rename to export password? 
    public partial class ExportPasswords : Form
    {
        private readonly SaveFileDialog _dialogue;
        private readonly List<StoredPassword> _passwords;
        private readonly IStreamWriterToEnd _streamWriter;

        private readonly UpdateListCallback _callback;
        private readonly Form _hidingBehind;

        public ExportPasswords(SaveFileDialog dialogue, List<StoredPassword> passwords, IStreamWriterToEnd streamWriter, UpdateListCallback callback, Form hidingBehind)
        {
            InitializeComponent();
            
            _passwords = passwords;
            _dialogue = dialogue;
            _streamWriter = streamWriter;

            _callback = callback;
            _hidingBehind = hidingBehind;

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Notify();

            if (string.IsNullOrEmpty(this.passwordBox.Text))
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

                Export(password);

                this.Close();
            }
        }

        private void Notify()
        {
            ActivityManager.Instance.Notify();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Notify();
            this.Close();
        }

        private void Export(SecureString password)
        {
            var exporter = new EncryptingPasswordExporter();

            var exportedJson = exporter.ExportPasswords(_passwords, password);

            _streamWriter.WriteToNewFile(_dialogue.FileName, exportedJson);
        }

        private void ViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _callback(OperationType.NoOperation, _hidingBehind);
        }

    }
}
