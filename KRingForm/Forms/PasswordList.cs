using KRingCore.Core.Model;
using KRingCore.Core.Services;
using KRingCore.Extensions;
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
        NoOperation = 7
    }

    public partial class PasswordList : Form
    {
        public delegate void UpdateListCallback(OperationType operation);
        public delegate void ExitingCallback();
        public delegate void ImportCallback(List<StoredPassword> passwords);
        public delegate void ErrorCallback(string messageToUser);

        private readonly User _user;
        private IStoredPasswordRepository _passwordRep;
        private readonly IPasswordImporter _passwordImporter;
        private readonly IProfileRepository _profileRepository;

        //TODO: restart once we have used it, it could be that the user makes more changes? 
        private Task<Tuple<SymmetricKey, SymmetricKey>> RehashingTask;

        private int _currentIndex;

        //TODO: gather this in a state object or a state design pattern or something.
        private bool _unsavedChanges;
        private bool _savedSoFar;
        private bool _exitWithoutSaving;

        private const int secondsToWait = 120;

        public PasswordList(User user, IProfileRepository profileRepository)
        {
            InitializeComponent();
            _user = user;
            _profileRepository = profileRepository;

            /* Start rehashing for potential re-encryption already now in a task since it takes quite a while */
            RehashingTask = Task.Run(() =>
            {
                this._user.GenerateNewSalt();
                return Tuple.Create(
                                        new SymmetricKey(this._user.Password, this._user.SecurityData.EncryptionKeySalt), 
                                        new SymmetricKey(this._user.Password, this._user.SecurityData.MacKeySalt)
                                    );
            });

            var securePassword = new SecureString();
            securePassword.PopulateWithString(user.PlaintextPassword);

            _passwordRep = new StoredPasswordRepository(securePassword, _user.SecurityData.EncryptionKeySalt, _user.SecurityData.MacKeySalt);
            _passwordImporter = new PlaintextPasswordImporter();

            UpdateList(OperationType.NoOperation);

            if (_passwordRep.DecryptionErrorOccured)
            {
                _passwordRep.DeleteAllEntries();
                string message = "One or more passwords were corrupted and could not be decrypted. They have thus been deleted";
                MessageBox.Show(message);
                Program.Log("PasswordList", message);
            }

            _unsavedChanges = false;
            _savedSoFar = false;
            HideSaveButton();
            _exitWithoutSaving = false;

            ActivityManager.Instance.Init(Notify);
            ActivityManager.Instance.Notify();
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

                var viewForm = new ViewForm(entry);
                viewForm.Show();
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
                var addForm = new AddPasswordForm(_passwordRep, UpdateList);
                addForm.Show();
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

                var editForm = new EditPasswordForm(_passwordRep, UpdateList, entry);
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

                if (string.IsNullOrEmpty(selectedDomain)) return;

                var deleteForm = new DeletePasswordForm(_passwordRep, UpdateList, selectedDomain);
                deleteForm.Show();
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
            Task reencrypTask = ReencryptAndSave();

            Notify();

            HideSaveButton();

            await reencrypTask;
        }


        private void SearchButton_Click(object sender, EventArgs e)
        {
            var searchString = SearchBar.Text;

            /* If search text is empty, show entire list */
            if (searchString == string.Empty || searchString == null)
            {
                UpdateList(OperationType.NoOperation);

                return;
            }

            /* Otherwise, do a prefix search on the domain names */
            var result = _passwordRep.PrefixSearch(searchString);

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

        private async Task ReencryptAndSave()
        {
            /* Wait for new salt and key derivation to complete */
            var rehashingResult = await RehashingTask;

            /* New password rep that derives and uses new keys, copy over existing passwords before overwriting. */
            var passwords = _passwordRep.GetEntries().ToList();
            _passwordRep = new StoredPasswordRepository(this._user.Password,
                                                        rehashingResult.Item1,
                                                        rehashingResult.Item2,
                                                        _passwordRep.GetEntries());


            /* Start write */
            await _passwordRep.WriteEntriesToDbAsync();

            if (_passwordRep.EncryptionErrorOccured)
            {
                Program._messageToUser("One or more passwords could not be encrypted - their data has been lost");
                Program.Log("Save", "One or more passwords could not be encrypted");
            }

            await _profileRepository.WriteUserAsync(this._user);

            _unsavedChanges = false;
            _savedSoFar = true;
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

            // TODO: when form closes, no matter what, we must:
            // Derive new salt for password and username hashing. 
            // Derive new salt for mac and encr key
            // Derive new set of keyts from above, and use that for encryption before writing
            // Save salt and other user values in profile (update), so it can be read next upstart
            // this gives somewhat forward secrecy.
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
            var importForm = new ImportPasswords(fileDialogue, ImportPasswords, DisplayErrorMessageToUser, new StreamReaderToEnd());
            importForm.Show();
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

                var infoDialogue = new InformationPopup(info);

                infoDialogue.Show();

                if (moreThanOne)
                {
                    onlyDuplicates = listOfDuplicates.Count == passwords.Count;
                }
            }

            if (!onlyDuplicates)
            {
                UpdateList(OperationType.AddPassword);
            }
            else
            {
                UpdateList(OperationType.NoOperation);
            }
        }
        
        private void SaveFile_Ok(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var dialogue = sender as SaveFileDialog;
            var exportPasswordForm = new ExportPasswords(dialogue, 
                                                        this._passwordRep.GetEntries(), 
                                                        new StreamWriterToEnd());
            exportPasswordForm.Show();
        }

        #endregion

        private void DisplayErrorMessageToUser(string messageToUser)
        {
            Program.Log("Displaying error to user", messageToUser);
            var informationForm = new InformationPopup(messageToUser);
            informationForm.Show();
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
