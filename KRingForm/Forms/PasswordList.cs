using KRing.Core.Model;
using KRing.Persistence.Interfaces;
using KRing.Persistence.Repositories;
using KRingForm.Forms;
using KRing.Persistence.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace KRingForm
{
    public enum OperationType
    {
        NewPassword = 0,
        ViewPassword = 1,
        AddPassword = 2,
        EditPassword = 3,
        DeletePassword = 4,
        SavePassword = 5,
        DeleteUser = 6,
        NoOperation = 7
    }

    public partial class PasswordList : Form
    {
        public delegate void UpdateListCallback(OperationType operation);
        public delegate void ExitingCallback();

        private readonly User _user;
        private readonly IStoredPasswordRepository _passwordRep;

        private int _currentIndex;
        private bool _unsavedChanges;
        private bool _exitWithoutSaving;

        public PasswordList(User user)
        {
            InitializeComponent();
            _user = user;

            _passwordRep = new StoredPasswordRepository(_user.Password);

            UpdateList(OperationType.NoOperation);

            if (_passwordRep.DecryptionErrorOccured)
            {
                MessageBox.Show("One or more passwords were corrupted and could not be decrypted. They have thus been deleted");
            }

            _unsavedChanges = false;
            _exitWithoutSaving = false;
        }

        public void NotExiting()
        {
            _exitWithoutSaving = false;
        }

        public void Exiting()
        {
            _exitWithoutSaving = true;
            this.Close();
        }

        /* TODO: make async? */
        public void UpdateList(OperationType operation)
        {
            passwordListBox.Items.Clear();

            var passwords = _passwordRep.GetEntries();

            foreach (var pswd in passwords)
            {
                passwordListBox.Items.Add(pswd.Domain);
            }

            if(operation != OperationType.SavePassword || operation != OperationType.NoOperation)
            {
                _unsavedChanges = true;
            }
            else
            {
                _unsavedChanges = false;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentIndex = passwordListBox.SelectedIndex;            
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            try
            {
                var addForm = new AddPasswordForm(_passwordRep, UpdateList);
                addForm.Show();
            }
            catch(Exception)
            {
                HandleException();
            }            
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedDomain = GetCurrentDomain(_currentIndex);

                var editForm = new EditPasswordForm(_passwordRep, UpdateList, selectedDomain);
                editForm.Show();
            }
            catch (Exception)
            {
                HandleException();
            }

        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedDomain = GetCurrentDomain(_currentIndex);

                var deleteForm = new DeletePasswordForm(_passwordRep, UpdateList, selectedDomain);
                deleteForm.Show();
            }
            catch (Exception)
            {
                HandleException();
            }

        }

        private string GetCurrentDomain(int index)
        {
            var list = GetListAsStrings();

            if(index >= list.Count)
            {
                throw new ArgumentException("Index out of range");
            }

            return list.ElementAt(index);
        }

        private List<string> GetListAsStrings()
        {
            return passwordListBox.Items.Cast<string>().ToList();
        }

        private void viewButton_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedDomain = GetCurrentDomain(_currentIndex);

                var password = _passwordRep.GetPasswordFromDomain(selectedDomain);

                var entry = new StoredPassword(selectedDomain, password);

                var viewForm = new ViewForm(entry);
                viewForm.Show();
            }
            catch (Exception)
            {
                HandleException();
            }

        }

        private void deleteUserButton_Click(object sender, EventArgs e)
        {
            //TODO
        }

        /// <summary>
        /// If something fails, make sure to write database.
        /// </summary>
        private void HandleException()
        {
            _passwordRep.WriteEntriesToDb();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            _passwordRep.WriteEntriesToDb();

            if(_passwordRep.EncryptionErrorOccured)
            {
                Program._messageToUser("One or more passwords could not be encrypted - their data has been lost");
            }

            _unsavedChanges = false;
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            try
            {
                var newForm = new NewPasswordForm(_passwordRep, UpdateList);
                newForm.Show();
            }
            catch (Exception)
            {
                HandleException();
            }
        }

        private void PasswordList_Load(object sender, EventArgs e)
        {

        }

        private void PasswordList_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_unsavedChanges && !_exitWithoutSaving)
            {
                e.Cancel = true;
                
                var warning = new ExitDialogue(
                    Exiting,
                    NotExiting);
                warning.Show();
            }
        }
    }
}
