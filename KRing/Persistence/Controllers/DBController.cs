using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using KRing.Core;
using KRing.Core.Model;
using KRing.DTO;
using KRing.Extensions;
using KRing.Interfaces;
using KRing.Persistence.Model;
using KRing.Persistence.Repositories;

namespace KRing.Persistence.Controllers
{ 
    public class DbController
    {
        /// <summary>
        /// Singleton pattern, since we use the same file throughout, so we only want one accessing it at any time. 
        /// </summary>
        private static DbController _instance;
        private static DbEntryRepository _dbEntryRepository;
        public int EntryCount => _dbEntryRepository.EntryCount;

        public static DbController Instance(SecureString password)
        {
            return 
                _instance ?? (_instance = new DbController(password));
        }

        private DbController(SecureString password)
        {
            _dbEntryRepository = new DbEntryRepository(password);
        }

        public void AddPassword(IUserInterface ui, Session session)
        {
            DbEntryDto newEntry = ui.RequestNewEntryInformation(session.User);
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

            _dbEntryRepository.ShowAllDomainsToUser(ui);

            var correctDomainGiven = false;
            var domain = String.Empty;

            while (!correctDomainGiven)
            {
                domain = ui.RequestUserInput("Please Enter Domain to Update");
                correctDomainGiven = _dbEntryRepository.ExistsEntry(domain);

                if (!correctDomainGiven) ui.MessageToUser("That Domain Does not exist amongst stored passwords");
            }

            var newPassword = ui.RequestPassword("Please enter new password for the domain " + domain);
            _dbEntryRepository.UpdateEntry(new DbEntryDto(domain, newPassword));
        }

        public void DeletePassword(IUserInterface ui)
        {
            if (EntryCount <= 0)
            {
                ui.MessageToUser("You have no passwords stored\n");
                return;
            }

            _dbEntryRepository.ShowAllDomainsToUser(ui);

            var correctDomainGiven = false;
            var domain = string.Empty;

            while (!correctDomainGiven)
            {
                domain = ui.RequestUserInput("Please Enter Domain to Delete");
                correctDomainGiven = _dbEntryRepository.ExistsEntry(domain);

                if (!correctDomainGiven) ui.MessageToUser("That domain does not exist amongst stored passwords");
            }

            _dbEntryRepository.DeleteEntry(domain);
        }

        public void ViewPassword(IUserInterface ui)
        {
            if (EntryCount <= 0)
            {
                ui.MessageToUser("You have no passwords stored\n");
                return;
            }

            bool correctDomainGiven = false;
            string domain = String.Empty;

            _dbEntryRepository.ShowAllDomainsToUser(ui);

            int entryCount = _dbEntryRepository.EntryCount;
            int requestedDomain = 0;

            while (!correctDomainGiven)
            {
                domain = ui.RequestUserInput("Please Enter index of Domain to get corresponding Password");

                
                Int32.TryParse(domain, out requestedDomain);

                correctDomainGiven = requestedDomain <= entryCount;

                if (!correctDomainGiven) ui.MessageToUser("That index does not exist, please try again");
            }

            try
            {
                int zeroIndexedDomain = requestedDomain - 1;
                var entry = _dbEntryRepository.GetPasswordFromCount(zeroIndexedDomain);
                ui.MessageToUser("Password for domain " + domain + " is:\n\n " + entry.ConvertToUnsecureString());
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
            catch(Exception e)
            {
                throw e;
            }
            
        }
        
        public void DeleteAllEntries()
        {
            _dbEntryRepository.DeleteAllEntries();
        }

        public void SaveAllEntries()
        {
            _dbEntryRepository.WriteEntriesToDb();
        }
    }
}
