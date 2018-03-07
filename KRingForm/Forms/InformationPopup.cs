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
    public partial class InformationPopup : Form
    {
        private readonly UpdateListCallback _callback;
        private readonly Form _hidingBehind;

        public InformationPopup(string information, UpdateListCallback callback, Form hidingBehind)
        {
            InitializeComponent();

            _callback = callback;
            _hidingBehind = hidingBehind;

            informationBox.Text = information;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void informationBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _callback(OperationType.NoOperation, _hidingBehind);
        }
    }
}
