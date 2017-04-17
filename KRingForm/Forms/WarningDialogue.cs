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

        private void yesButton_Click(object sender, EventArgs e)
        {
            _close();
            this.Close();
        }

        private void noButton_Click(object sender, EventArgs e)
        {
            _open();
            this.Close();
        }
    }
}
