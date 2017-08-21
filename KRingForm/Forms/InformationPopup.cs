using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KRingForm.Forms
{
    public partial class InformationPopup : Form
    {
        public InformationPopup(string information)
        {
            InitializeComponent();

            informationBox.Text = information;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
