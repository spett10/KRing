using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KRing.Persistence.Model;
using KRing.Extensions;

namespace KRingForm.Forms
{
    public partial class ViewForm : Form
    {
        private readonly StoredPassword _entry;
        private DateTime _startTime;
        private DateTime _endTime;
        private static readonly int _secondsToDisplay = 5;

        private readonly string _password;

        private string message = "Window closes after " + _secondsToDisplay.ToString() + "seconds";

        public ViewForm(StoredPassword entry)
        {
            InitializeComponent();

            _entry = entry;

            domainBox.Text = _entry.Domain;           

            _password = entry.PlaintextPassword;
            passwordBox.Text = string.Concat(Enumerable.Repeat("*", _password.Length));
            warning.Hide();
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(_password);
        }

        private void warningTimer_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void revealButton_Click(object sender, EventArgs e)
        {
            _startTime = DateTime.Now;
            _endTime = _startTime.AddSeconds(_secondsToDisplay);

            this.passwordBox.Text = _password;

            warningTimer.Interval = (int)(_endTime - _startTime).TotalMilliseconds;
            warningTimer.Start();

            warning.Show();
        }
    }
}
