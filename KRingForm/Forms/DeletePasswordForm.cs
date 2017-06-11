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
        private readonly ActiveCallback _activity;

        public DeletePasswordForm(IStoredPasswordRepository repository, UpdateListCallback callback, ActiveCallback activity, string entry)
        {
            InitializeComponent();

            _repository = repository;
            _entry = entry;
            _callback = callback;
            _activity = activity;

            domainBox.Text = entry;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Notify();
            _repository.DeleteEntry(_entry);
            _callback(OperationType.DeletePassword);
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Notify();
            this.Close();
        }

        private void Notify()
        {
            _activity();
        }
    }
}
