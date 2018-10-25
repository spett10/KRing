using KRingCore.Core.Model;
using KRingCore.Core.Services;
using Krypto.Extensions;
using KRingCore.Persistence.Interfaces;
using KRingCore.Persistence.Model;
using KRingCore.Persistence.Repositories;
using KRingForm.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        RefreshList = 7,
        NoOperation = 8
    }

    public partial class PasswordList : Form
    {
        public delegate void UpdateListCallback(OperationType operation, Form disabled);
        public delegate Task ExitingCallback();
        public delegate void ImportCallback(List<StoredPassword> passwords);
        public delegate void ErrorCallback(string messageToUser);

        private readonly User _user;
        private IStoredPasswordRepository _passwordRep;
        private readonly IProfileRepository _profileRepository;
        
        private int _currentIndex;

        //TODO: gather this in a state object or a state design pattern or something.
        private bool _unsavedChanges;
        private bool _savedSoFar;
        private bool _exitWithoutSaving;
        private bool _userExiting;

        private const int secondsToWait = 500;
        
        public PasswordList(User user, IProfileRepository profileRepository)
        {
            InitializeComponent();
            _user = user;
            _profileRepository = profileRepository;

            this._user.GenerateNewSalt();

            var securePassword = new SecureString();
            securePassword.PopulateWithString(user.PlaintextPassword);

            _passwordRep = new StoredPasswordRepository(securePassword);

            UpdateList(OperationType.RefreshList, this);

            if (_passwordRep.DecryptionErrorOccured)
            {
                _passwordRep.DeleteAllEntries();
                string message = "Database could not be decrypted, and/or it's integrity had been compromised. Database deleted.";
                MessageBox.Show(message);
                Program.Log("PasswordList", message);
            }

            _unsavedChanges = false;
            _savedSoFar = false;
            HideSaveButton();
            _exitWithoutSaving = false;
            _userExiting = true;

            ActivityManager.Instance.Init(Notify);
            ActivityManager.Instance.Notify();
        }

        public async Task NotExiting()
        {
            _exitWithoutSaving = false;
        }

        public async Task ExitingWithoutSaving()
        {
            _exitWithoutSaving = true;

            if (!_savedSoFar)
            {
                /* Since there are unsaved changes we want to discard, but still want to reencrypt what we had before changes, we reload values from file, and then store again */
                var entries = this._passwordRep.LoadEntriesFromDb();

                await ReencryptAndSave(entries);
            }

            this.Close();
        }

        /* todo: This logic is ugly. Plus, other functionality (refrehs list) depends on this tightly. Can we somehow refactor? */
        public void UpdateList(OperationType operation, Form disabled)
        {
            /* we always disable something prior to this callback */
            disabled.Enabled = true;

            if(operation == OperationType.NoOperation)
            {
                Notify();
                return;
            }

            passwordListBox.Items.Clear();

            var passwords = _passwordRep.GetEntries();

            foreach (var pswd in passwords)
            {
                passwordListBox.Items.Add(pswd.Domain);
            }

            var operationsWithNoChanges = new List<OperationType>() { OperationType.SavePassword, OperationType.NoOperation, OperationType.RefreshList };

            if(!operationsWithNoChanges.Contains(operation))
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

        public void Notify()
        {
            ResetInactiveTimer();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentIndex = passwordListBox.SelectedIndex;
            Notify();
        }


        /// <summary>
        /// Returns empty string is the index is out of range - dont throw errors as logic control, just check when calling. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private string GetCurrentDomain(int index)
        {
            var list = GetListAsStrings();

            if(index >= list.Count || index < 0)
            {
                return string.Empty;
            }

            return list.ElementAt(index);
        }

        private List<string> GetListAsStrings()
        {
            return passwordListBox.Items.Cast<string>().ToList();
        }

        #region ButtonEventHandlers

        private void viewButton_Click(object sender, EventArgs e)
        {
            Notify();

            try
            {
                var selectedDomain = GetCurrentDomain(_currentIndex);

                if (string.IsNullOrEmpty(selectedDomain)) return;

                var entry = _passwordRep.GetEntry(selectedDomain);

                FormArranger.HideBehind(this, new ViewForm(entry, UpdateList, this));
            }
            catch (Exception)
            {
                HandleException();
                Program.Log("viewButton", "Exception occured");
            }

        }

        private void addButton_Click(object sender, EventArgs e)
        {
            Notify();

            try
            {
                FormArranger.HideBehind(this, new AddPasswordForm(_passwordRep, UpdateList, this));
            }
            catch (Exception)
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

                if (string.IsNullOrEmpty(selectedDomain)) return;

                var entry = _passwordRep.GetEntry(selectedDomain);

                FormArranger.HideBehind(this, new EditPasswordForm(_passwordRep, UpdateList, entry, this));
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

                if (string.IsNullOrEmpty(selectedDomain)) return;

                FormArranger.HideBehind(this, new DeletePasswordForm(_passwordRep, UpdateList, selectedDomain, this));
            }
            catch (Exception)
            {
                HandleException();
                Program.Log("deleteButton", "Exception occured");
            }

        }


        private void deleteUserButton_Click(object sender, EventArgs e)
        {
            //TODO
            //Cleanup password repository
            //Cleanup profile
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

        private void importButton_Click(object sender, EventArgs e)
        {
            var fileDialogue = new OpenFileDialog();
            fileDialogue.FileOk += FileDialogue_FileOk;
            fileDialogue.ShowDialog();
        }

        private async void saveButton_Click(object sender, EventArgs e)
        {
            this.Enabled = false;

            Task reencrypTask = ReencryptAndSave(_passwordRep.GetEntries().ToList());

            Notify();

            HideSaveButton();

            await reencrypTask;

            _savedSoFar = true;
            _unsavedChanges = false;

            this.Enabled = true;
        }


        private void SearchButton_Click(object sender, EventArgs e)
        {
            var searchString = SearchBar.Text;

            /* If search text is empty, show entire list */
            if (String.IsNullOrEmpty(searchString))
            {
                UpdateList(OperationType.RefreshList, this);

                return;
            }

            /* Otherwise, do a contains search on the domain names */
            var result = _passwordRep.ContainsSearch(searchString);

            UpdateList(result);
        }

        #endregion


        /// <summary>
        /// If something fails, make sure to write database. Its a blocking call, we want to be really sure. 
        /// TODO: should this be a reencrpy thingie?
        /// </summary>
        private void HandleException()
        {
            Program.Log("PasswordList", "Exception occured, trying to write db");
            _passwordRep.WriteEntriesToDb();
        }

        private async Task ReencryptAndSave(List<StoredPassword> passwords)
        {
            /* New password rep that derives and uses new keys, copy over existing passwords before overwriting. */
            /* todo: do we need a new one? because the io will start deriving keys after each write. */
            _passwordRep.ReplaceEntries(passwords);


            /* Start write */
            await _passwordRep.WriteEntriesToDbAsync();

            if (_passwordRep.EncryptionErrorOccured)
            {
                Program._messageToUser("One or more passwords could not be encrypted - their data has been lost");
                Program.Log("Save", "One or more passwords could not be encrypted");
            }
        }

        private void PasswordList_Load(object sender, EventArgs e)
        {

        }

        private async void PasswordList_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_userExiting)
            {
                // do the saving stuff, then call this.close
                e.Cancel = true;

                if (_unsavedChanges && !_exitWithoutSaving && !Program.userInactiveLogout)
                {
                    var warning = new ExitDialogue(
                        ExitingWithoutSaving,
                        NotExiting);
                    warning.Show();
                }
                else if (!_unsavedChanges && !_savedSoFar)
                {
                    if (!_savedSoFar || Program.userInactiveLogout)
                    {
                        /* No unsaved changes, but we havent saved at all yet - so reencrypt */
                        var entries = this._passwordRep.GetEntries();

                        await ReencryptAndSave(entries);
                    }
                }
                _userExiting = false;
                this.Close(); //Call this function again, but now we skip the resave logic. 
            }
            else
            {
                // we are calling ourselves from the above, so just exit. 
            }
        }

        #region Timer

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
#if DEBUG

            Program.userInactiveLogout = false;
#else
            Program.userInactiveLogout = true;
#endif
            this.Close();
        }

        #endregion

        #region ExportImportLogic

        private void FileDialogue_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var fileDialogue = sender as OpenFileDialog;

            FormArranger.HideBehind(this, 
                                    new ImportPasswords(fileDialogue, 
                                                        ImportPasswords, 
                                                        DisplayErrorMessageToUser, 
                                                        new StreamReaderToEnd(), 
                                                        UpdateList, 
                                                        this));
        }

        private void ImportPasswords(List<StoredPassword> passwords)
        {
            bool duplicateFound = false;

            var listOfDuplicates = new List<string>();

            foreach (var entry in passwords)
            {
                System.Diagnostics.Debug.WriteLine(entry.Domain + ", " + entry.Username + ", " + entry.PlaintextPassword);
                try
                {
                    _passwordRep.AddEntry(entry);
                }
                catch (ArgumentException)
                {
                    listOfDuplicates.Add(entry.Domain);
                    duplicateFound = true;
                }
            }

            var onlyDuplicates = false;

            if (duplicateFound)
            {
                string info = "One or more domains were found to be duplicates. This is not allowed. The duplicates were: ";

                bool moreThanOne = listOfDuplicates.Count > 1;
                bool lastYet = false;
                int count = 1;

                foreach (var str in listOfDuplicates)
                {
                    info += str;
                    lastYet = listOfDuplicates.Count == count;
                    if (moreThanOne && !lastYet)
                    {
                        info += ", ";
                    }

                    count++;
                }
                
                FormArranger.HideBehind(this, new InformationPopup(info, UpdateList, this));

                if (moreThanOne)
                {
                    onlyDuplicates = listOfDuplicates.Count == passwords.Count;
                }
            }

            if (!onlyDuplicates)
            {
                UpdateList(OperationType.AddPassword, this);
            }
            else
            {
                UpdateList(OperationType.NoOperation, this);
            }
        }
        
        private void SaveFile_Ok(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var dialogue = sender as SaveFileDialog;
            FormArranger.HideBehind(this, new ExportPasswords(dialogue,
                                                        this._passwordRep.GetEntries(),
                                                        new StreamWriterToEnd(),
                                                        UpdateList,
                                                        this));
        }

        #endregion

        private void DisplayErrorMessageToUser(string messageToUser)
        {
            Program.Log("Displaying error to user", messageToUser);
            FormArranger.HideBehind(this, new InformationPopup(messageToUser, UpdateList, this));
        }
    }

    /// <summary>
    /// Add a layer of indirection so we dont point to passwordlist.notify from all other forms, but to this one place, so its easier to change if needed. 
    /// </summary>
    public class ActivityManager
    {
        private static ActivityManager _instance;

        private ActiveCallback _callback;

        private ActivityManager()
        {

        }

        public static ActivityManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new ActivityManager();
                }
                return _instance;
            }
        }

        public void Init(ActiveCallback callback)
        {
            _callback = callback;
        }

        public void Notify()
        {
            _callback?.Invoke();
        }
    }
}
