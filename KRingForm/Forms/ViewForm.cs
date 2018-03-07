using System;
using System.Linq;
using System.Windows.Forms;
using KRingCore.Persistence.Model;
using KRingCore.Core;
using static KRingForm.PasswordList;

namespace KRingForm.Forms
{
    public partial class ViewForm : Form
    {
        private readonly StoredPassword _entry;

        private DateTime _revealStartTime;
        private DateTime _revealEndTime;

        private DateTime _clipboardStartTime;
        private DateTime _clipboardEndTime;

        private static readonly int _secondsToDisplay = 5;
        private static readonly int _secondsOnClipboard = 10;

        private readonly string _password;

        private string revealMessage = "Window closes after " + _secondsToDisplay.ToString() + "seconds";
        private string clipboardMessage = "Clipboard erased after " + _secondsOnClipboard.ToString() + "seconds";

        private bool _isPasswordCopied;

        private readonly UpdateListCallback _callback;
        private readonly Form _hidingBehind;

        public ViewForm(StoredPassword entry, UpdateListCallback callback, Form hidingBehind)
        {
            InitializeComponent();
            
            _entry = entry;

            domainBox.Text = _entry.Domain;
            usernameBox.Text = _entry.Username;

            _isPasswordCopied = false;
            _callback = callback;
            _hidingBehind = hidingBehind;

            _password = entry.PlaintextPassword;
            passwordBox.Text = string.Concat(Enumerable.Repeat("*", _password.Length));
            warning.Hide();
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(_password);

            _isPasswordCopied = true;

            _clipboardStartTime = DateTime.Now;
            _clipboardEndTime = _clipboardStartTime.AddSeconds(_secondsOnClipboard);

            clipboardTimer.Interval = (int)(_clipboardEndTime - _clipboardStartTime).TotalMilliseconds;
            clipboardTimer.Start();

            warning.Text = clipboardMessage;
            warning.Show();

            Notify();
        }

        private void warningTimer_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void revealButton_Click(object sender, EventArgs e)
        {
            _revealStartTime = DateTime.Now;
            _revealEndTime = _revealStartTime.AddSeconds(_secondsToDisplay);

            this.passwordBox.Text = _password;

            warningTimer.Interval = (int)(_revealEndTime - _revealStartTime).TotalMilliseconds;
            warningTimer.Start();

            warning.Text = revealMessage;
            warning.Show();

            Notify();
        }

        private void clipboardTimer_Tick(object sender, EventArgs e)
        {
            Clipboard.SetText(" ");
        }

        private void Notify()
        {
            ActivityManager.Instance.Notify();
        }

        private void ViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_isPasswordCopied)
            {
                Clipboard.SetText(" ");
            }

            _callback(OperationType.NoOperation, _hidingBehind);
        }
    }
}
