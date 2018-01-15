using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static KRingForm.PasswordList;

namespace KRingForm.Forms
{
    public partial class ExitDialogue : Form
    {
        private ExitingCallback _close;
        private ExitingCallback _open;

        public ExitDialogue(ExitingCallback close, ExitingCallback open)
        {
            InitializeComponent();

            _close = close;
            _open = open;

            _open();
        }

        private async void yesButton_Click(object sender, EventArgs e)
        {
            await _close();
            this.Close();
        }

        private async void noButton_Click(object sender, EventArgs e)
        {
            await _open();
            this.Close();
        }
    }
}
