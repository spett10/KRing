using KRingCore.Core.Model;
using KRingCore.Persistence.Interfaces;
using KRingCore.Persistence.Repositories;
using KRingForm.Forms;
using KRingCore.Persistence.Model;
using KRingCore.Extensions;
using KRingCore.Core.Services;
using KRingCore.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Security;
using System.Windows.Controls;

//TODO: order eventhandlers, and other stuff, under regions. 

namespace KRingForm
{
    public delegate void ActiveCallback();

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
        private readonly IPasswordImporter _passwordImporter; 

        private int _currentIndex;
        private bool _unsavedChanges;
        private bool _exitWithoutSaving;

        private const int secondsToWait = 120;

        public PasswordList(User user)
        {
            InitializeComponent();
            _user = user;

            var securePassword = new SecureString();
            securePassword.PopulateWithString(user.PlaintextPassword);

            _passwordRep = new StoredPasswordRepository(securePassword, _user.Cookie.EncryptionKeySalt, _user.Cookie.MacKeySalt);
            _passwordImporter = new PlaintextPasswordImporter();

            UpdateList(OperationType.NoOperation);

            if (_passwordRep.DecryptionErrorOccured)
            {
                string message = "One or more passwords were corrupted and could not be decrypted. They have thus been deleted";
                MessageBox.Show(message);
                Program.Log("PasswordList", message);
            }

            _unsavedChanges = false;
            HideSaveButton();
            _exitWithoutSaving = false;

            Notify();
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

            if(operation != OperationType.SavePassword && operation != OperationType.NoOperation)
            {
                _unsavedChanges = true;
                ShowSaveButton();
            }
            else
            {
                _unsavedChanges = false;
                HideSaveButton();
            }

            Notify();
        }

        public void UpdateList(IEnumerable<StoredPassword> searchResult)
        {
            passwordListBox.Items.Clear();

            foreach(var pswd in searchResult)
            {
                passwordListBox.Items.Add(pswd.Domain);
            }

            Notify();
        }

        private void HideSaveButton()
        {
            this.saveButton.Enabled = false;
        }

        private void ShowSaveButton()
        {
            this.saveButton.Enabled = true;
        }

        private void Notify()
        {
            ResetInactiveTimer();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentIndex = passwordListBox.SelectedIndex;
            Notify();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            Notify();

            try
            {
                var addForm = new AddPasswordForm(_passwordRep, UpdateList, Notify);
                addForm.Show();
            }
            catch(Exception)
            {
                HandleException();
                Program.Log("addButton", "Exception occured");
            }            
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            Notify();

            try
            {
                var selectedDomain = GetCurrentDomain(_currentIndex);

                var entry = _passwordRep.GetEntry(selectedDomain);

                var editForm = new EditPasswordForm(_passwordRep, UpdateList, Notify, entry);
                editForm.Show();
            }
            catch (Exception)
            {
                HandleException();
                Program.Log("editButton", "Exception occured");
            }

        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            Notify();

            try
            {
                var selectedDomain = GetCurrentDomain(_currentIndex);

                var deleteForm = new DeletePasswordForm(_passwordRep, UpdateList, Notify, selectedDomain);
                deleteForm.Show();
            }
            catch (Exception)
            {
                HandleException();
                Program.Log("deleteButton", "Exception occured");
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
            Notify();

            try
            {
                var selectedDomain = GetCurrentDomain(_currentIndex);

                var entry = _passwordRep.GetEntry(selectedDomain);

                var viewForm = new ViewForm(entry, Notify);
                viewForm.Show();
            }
            catch (Exception)
            {
                HandleException();
                Program.Log("viewButton", "Exception occured");
            }

        }

        private void deleteUserButton_Click(object sender, EventArgs e)
        {
            //TODO
        }

        /// <summary>
        /// If something fails, make sure to write database. Its a blocking call, we want to be really sure. 
        /// </summary>
        private void HandleException()
        {
            Program.Log("PasswordList", "Exception occured, trying to write db");
            _passwordRep.WriteEntriesToDb();
        }

        private async void saveButton_Click(object sender, EventArgs e)
        {
            Notify();

            /* Start write */
            await _passwordRep.WriteEntriesToDbAsync();

            if(_passwordRep.EncryptionErrorOccured)
            {
                Program._messageToUser("One or more passwords could not be encrypted - their data has been lost");
                Program.Log("Save", "One or more passwords could not be encrypted");
            }

            _unsavedChanges = false;
            HideSaveButton();
        }

        private void PasswordList_Load(object sender, EventArgs e)
        {

        }

        private void PasswordList_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_unsavedChanges && !_exitWithoutSaving && !Program.userInactiveLogout)
            {
                e.Cancel = true;
                
                var warning = new ExitDialogue(
                    Exiting,
                    NotExiting);
                warning.Show();
            }
        }

        private void ResetInactiveTimer()
        {
            var startTime = DateTime.Now;
            var endTime = DateTime.Now.AddSeconds(secondsToWait);

            inactiveTimer.Interval = (int)(endTime - startTime).TotalMilliseconds;
            inactiveTimer.Start();
        }

        private void inactiveTimer_Tick(object sender, EventArgs e)
        {
            if (_unsavedChanges)
            {
                _passwordRep.WriteEntriesToDb();
            }

            Program.userInactiveLogout = true;

            this.Close();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            var searchString = SearchBar.Text;

            /* If search text is empty, show entire list */
            if(searchString == string.Empty || searchString == null)
            {
                UpdateList(OperationType.NoOperation);

                return;
            }

            /* Otherwise, do a prefix search on the domain names */
            var result = _passwordRep.PrefixSearch(searchString);

            UpdateList(result);
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            var fileDialogue = new OpenFileDialog();
            fileDialogue.FileOk += FileDialogue_FileOk;
            fileDialogue.ShowDialog();
        }

        private void FileDialogue_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var fileDialogue = sender as OpenFileDialog;

            var importedPasswords = _passwordImporter.ImportPasswords(fileDialogue.FileName);
            Program.Log("Import Passwords", "Imported passwords, count: " + importedPasswords.Count);

            bool duplicateFound = false;

            var listOfDuplicates = new List<string>();

            foreach(var entry in importedPasswords)
            {
                System.Diagnostics.Debug.WriteLine(entry.Domain + ", " + entry.Username + ", " + entry.PlaintextPassword);
                try
                {
                    _passwordRep.AddEntry(entry);
                }
                catch(ArgumentException)
                {
                    listOfDuplicates.Add(entry.Domain);
                    duplicateFound = true;
                }
            }

            var onlyDuplicates = false;

            if(duplicateFound)
            {
                string info = "One or more domains were found to be duplicates. This is not allowed. The duplicates were: ";

                bool moreThanOne = listOfDuplicates.Count > 1;
                bool lastYet = false;
                int count = 1;

                foreach(var str in listOfDuplicates)
                {
                    info += str;
                    lastYet = listOfDuplicates.Count == count;
                    if (moreThanOne && !lastYet)
                    {
                        info += ", ";
                    }

                    count++;
                }

                var infoDialogue = new InformationPopup(info);

                infoDialogue.Show();

                if (moreThanOne)
                {
                    onlyDuplicates = listOfDuplicates.Count == importedPasswords.Count;
                }
            }

            if(!onlyDuplicates)
            {
                UpdateList(OperationType.AddPassword);
            }
            else
            {
                UpdateList(OperationType.NoOperation);
            }
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            //TODO: i think the flow would be better if we first ask for password, then for the file? seems more natural. 

            var saveFileDialogue = new SaveFileDialog();
            saveFileDialogue.Filter = "Text File (*.txt)|*.txt";
            saveFileDialogue.Title = "Export passwords encrypted";
            saveFileDialogue.FileOk += SaveFile_Ok;
            saveFileDialogue.ShowDialog();
        }

        private void SaveFile_Ok(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var dialogue = sender as SaveFileDialog;
            var exportPasswordForm = new ExportPasswords(dialogue, 
                                                        this._passwordRep.GetEntries(), 
                                                        Notify, 
                                                        new StreamWriterToEnd());
            exportPasswordForm.Show();
        }
    }
}
