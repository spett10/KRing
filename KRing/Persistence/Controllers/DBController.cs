using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using KRing.Core;
using KRing.Core.Model;
using KRing.Extensions;
using KRing.Interfaces;
using KRing.Persistence.Model;
using KRing.Persistence.Repositories;
using KRing.Persistence.Interfaces;

namespace KRing.Persistence.Controllers
{ 
    public class DbController
    {
        /// <summary>
        /// Singleton pattern, since we use the same file throughout, so we only want one accessing it at any time. 
        /// </summary>
        private static DbController _instance;
        private static IDbEntryRepository _dbEntryRepository;

        public int EntryCount => _dbEntryRepository.EntryCount;

        public DbController(IDbEntryRepository dbEntryRepository)
        {
            _dbEntryRepository = dbEntryRepository;
        }

        public void AddPassword(IUserInterface ui, User user)
        {
            DBEntry newEntry = ui.RequestNewEntryInformation(user);
            try
            {
                _dbEntryRepository.AddEntry(newEntry);
            }
            catch (ArgumentException e)
            {
                ui.MessageToUser("\n" + e.Message);
            }
        }

        public void UpdatePassword(IUserInterface ui)
        {
            if (EntryCount <= 0)
            {
                ui.MessageToUser("You have no passwords stored\n");
                return;
            }

            int domain = GetIndexFromUser(ui);

            var entry = _dbEntryRepository.GetEntry(domain);
            
            var newPassword = PasswordAdvisor.CheckPasswordWithUserInteraction(ui);

            try
            {
                _dbEntryRepository.UpdateEntry(new DBEntry(entry.Domain, newPassword));
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
                ui.MessageToUser("Password for domain is:\n\n " + entry.ConvertToUnsecureString());
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
