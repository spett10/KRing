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

namespace KRingForm
{
    public partial class PasswordList : Form
    {
        public delegate void UpdateListCallback();

        private readonly User _user;
        private readonly IDbEntryRepository _passwordRep;

        private int _currentIndex;

        public PasswordList(User user)
        {
            InitializeComponent();
            _user = user;

            _passwordRep = new DbEntryRepository(_user.Password);

            UpdateList();

            if (_passwordRep.DecryptionErrorOccured)
            {
                MessageBox.Show("One or more passwords were corrupted and could not be decrypted. They have thus been deleted");
            }
        }

        /* TODO: make async? */
        public void UpdateList()
        {
            passwordListBox.Items.Clear();

            var passwords = _passwordRep.GetEntries();

            foreach (var pswd in passwords)
            {
                passwordListBox.Items.Add(pswd.Domain);
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

                var entry = new DBEntry(selectedDomain, password);

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
                MessageBox.Show("One or more passwords could not be encrypted - their data has been lost");
            }
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
    }
}
