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

        public static DbController Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new DbController();
                }

                return _instance;
            }
        }

        private DbController()
        {
            _dbEntryRepository = new DbEntryRepository();
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

            while (!correctDomainGiven)
            {
                domain = ui.RequestUserInput("Please Enter Domain to get corresponding Password");
                correctDomainGiven = _dbEntryRepository.ExistsEntry(domain);

                if (!correctDomainGiven) ui.MessageToUser("That Domain Does not exist amongst stored passwords");
            }

            var entry = _dbEntryRepository.GetPasswordFromEntry(domain);

            ui.MessageToUser("Password for domain " + domain + " is:\n\n " + entry.ConvertToUnsecureString());
        }

        public void LoadDb()
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

        /* fjern alle nedenunder efter vi har flyttet logikken fra program over */
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
