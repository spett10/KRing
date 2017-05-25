using System;
using KRingCore.Core;
using KRingCore.Core.Model;
using KRingCore.Extensions;
using KRing.Interfaces;
using KRingCore.Persistence.Model;
using KRingCore.Persistence.Interfaces;
using KRingCore.Core.Interfaces;

namespace KRing.Persistence.Controllers
{ 
    public class StoredPasswordController
    {
        /// <summary>
        /// Singleton pattern, since we use the same file throughout, so we only want one accessing it at any time. 
        /// </summary>
        private static IStoredPasswordRepository _dbEntryRepository;

        public int EntryCount => _dbEntryRepository.EntryCount;

        public StoredPasswordController(IStoredPasswordRepository dbEntryRepository)
        {
            _dbEntryRepository = dbEntryRepository;
        }

        public void AddPassword(IUserInterface ui, User user)
        {
            StoredPassword newEntry = ui.RequestNewEntryInformation(user);
            try
            {
                _dbEntryRepository.AddEntry(newEntry);
            }
            catch (ArgumentException e)
            {
                ui.MessageToUser("\n" + e.Message);
            }
        }

        public void UpdatePassword(IUserInterface ui, IPasswordUI pswdUi)
        {
            if (EntryCount <= 0)
            {
                ui.MessageToUser("You have no passwords stored\n");
                return;
            }

            int domain = GetIndexFromUser(ui);

            var entry = _dbEntryRepository.GetEntry(domain);
            
            var newPassword = PasswordAdvisor.CheckPasswordWithUserInteraction(pswdUi);

            try
            {
                _dbEntryRepository.UpdateEntry(new StoredPassword(entry.Domain, newPassword.ConvertToUnsecureString()));
            }
            catch(Exception)
            {
                ui.MessageToUser("Internal Error");
            }
        }

        public void DeletePassword(IUserInterface ui)
        {
            if (EntryCount <= 0)
            {
                ui.MessageToUser("You have no passwords stored\n");
                return;
            }

            int domain = GetIndexFromUser(ui);

            try
            { 
                _dbEntryRepository.DeleteEntry(domain);
            }
            catch(Exception)
            {
                ui.MessageToUser("Internal Error");
            }
        }

        public void ViewPassword(IUserInterface ui)
        {
            if (EntryCount <= 0)
            {
                ui.MessageToUser("You have no passwords stored\n");
                return;
            }

            int requestedDomain = GetIndexFromUser(ui);

            try
            {
                var entry = _dbEntryRepository.GetPasswordFromCount(requestedDomain);
                ui.MessageToUser("Password for domain is:\n\n " + entry);
            }
            catch(Exception)
            {
                ui.MessageToUser("Internal Error");
            }
        }

        public void LoadDb()
        {
            try
            {
                if (!_dbEntryRepository.IsDbEmpty())
                {
                    _dbEntryRepository.LoadEntriesFromDb();
                }
                else
                {
                    throw new AppDomainUnloadedException("No Db entries to load");
                }
            }
            catch(Exception)
            {
                throw;
            }
            
        }
        
        public void DeleteAllEntries()
        {
            _dbEntryRepository.DeleteAllEntries();
        }

        public void SaveAllEntries()
        {
            if (EntryCount > 0)
            {
                _dbEntryRepository.WriteEntriesToDb();
            } else
            {
                throw new Exception("No Entries To Save");
            }
        }

        private int GetIndexFromUser(IUserInterface ui)
        {
            bool correctDomainGiven = false;
            string domain = String.Empty;

            ui.ShowAllDomainsToUser(_dbEntryRepository.GetEntries());

            int entryCount = _dbEntryRepository.EntryCount;
            int requestedDomain = 0;

            while (!correctDomainGiven)
            {
                domain = ui.RequestUserInput("Please Enter index of Domain for the action");

                Int32.TryParse(domain, out requestedDomain);

                correctDomainGiven = requestedDomain <= entryCount;

                if (!correctDomainGiven) ui.MessageToUser("That index does not exist, please try again");
            }

            return requestedDomain - 1; //zero index
        }
    }
}
