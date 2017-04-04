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
        private readonly DBEntry _entry;
        private readonly DateTime _startTime;
        private readonly DateTime _endTime;
        private static readonly int _secondsToDisplay = 5;

        private string message = "Window closes after " + _secondsToDisplay.ToString() + "seconds";

        public ViewForm(DBEntry entry)
        {
            InitializeComponent();

            _entry = entry;

            _startTime = DateTime.Now;
            _endTime = _startTime.AddSeconds(_secondsToDisplay);

            this.domainBox.Text = entry.Domain;
            this.passwordBox.Text = entry.Password.ConvertToUnsecureString();

            entry.Password = new System.Security.SecureString();
            entry.Password.PopulateWithString(passwordBox.Text);

            warningTimer.Interval = (int)(_endTime - _startTime).TotalMilliseconds;
            warningTimer.Start();
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(passwordBox.Text);
        }

        private void warningTimer_Tick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
