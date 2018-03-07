using System;
using System.Windows.Forms;
using KRingCore.Persistence.Interfaces;
using static KRingForm.PasswordList;

namespace KRingForm.Forms
{
    public partial class DeletePasswordForm : Form
    {
        private readonly IStoredPasswordRepository _repository;
        private readonly string _entry;
        private readonly UpdateListCallback _callback;
        private readonly Form _hidingBehind;

        public DeletePasswordForm(IStoredPasswordRepository repository, UpdateListCallback callback, string entry, Form hidingBehind)
        {
            InitializeComponent();

            _repository = repository;
            _entry = entry;
            _callback = callback;
            _hidingBehind = hidingBehind;

            domainBox.Text = entry;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Notify();
            _repository.DeleteEntry(_entry);
            _callback(OperationType.DeletePassword, _hidingBehind);
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Notify();
            this.Close();
        }

        private void ViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _callback(OperationType.NoOperation, _hidingBehind);
        }

        private void Notify()
        {
            ActivityManager.Instance.Notify();
        }


    }
}
