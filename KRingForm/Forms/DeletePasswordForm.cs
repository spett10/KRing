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
using KRing.Persistence.Interfaces;
using static KRingForm.PasswordList;

namespace KRingForm.Forms
{
    public partial class DeletePasswordForm : Form
    {
        private readonly IStoredPasswordRepository _repository;
        private readonly string _entry;
        private readonly UpdateListCallback _callback;

        public DeletePasswordForm(IStoredPasswordRepository repository, UpdateListCallback callback, string entry)
        {
            InitializeComponent();

            _repository = repository;
            _entry = entry;
            _callback = callback;

            domainBox.Text = entry;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            _repository.DeleteEntry(_entry);
            _callback(OperationType.DeletePassword);
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
