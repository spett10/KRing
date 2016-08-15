using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using KRing.Core;
using KRing.DTO;
using KRing.Extensions;
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
        public List<DBEntry> Entries { get; private set; }
        public int EntryCount => Entries.Count;

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
            Entries = !_dbEntryRepository.IsDbEmpty() ? _dbEntryRepository.LoadEntriesFromDb() : new List<DBEntry>();
        }

        public void LoadEntries()
        {
            Entries = _dbEntryRepository.LoadEntriesFromDb();
        }

        public void DeleteEntryFromDomain(string domain)
        {
            var entry = Entries.SingleOrDefault(e => e.Domain.Equals(domain));
            if (entry != null)
            {
                Entries.Remove(entry);
            }
            
        }

        public void AddEntry(DbEntryDto newDto)
        {
            bool duplicateExists = Entries.Exists(
                                            e => e.
                                            Domain.
                                            Equals(newDto.Domain, StringComparison.OrdinalIgnoreCase));

            if (!duplicateExists)
            {
                DBEntry newEntry = new DBEntry(newDto.Domain, newDto.Password);
                Entries.Add(newEntry);
            }
            else
            {
                throw new ArgumentException("Error: Domain Already Exists");
            }
        }

        public void UpdateEntry(DbEntryDto updatedEntry)
        {
            var entry =
                Entries.FirstOrDefault(e => e.Domain.Equals(updatedEntry.Domain, StringComparison.OrdinalIgnoreCase));
            if (entry != null) entry.Password = updatedEntry.Password;
        }

        public bool ExistsEntry(string domain)
        {
            return Entries.Any(e => e.
                               Domain.
                               ToString().
                               Equals(domain, StringComparison.OrdinalIgnoreCase));
        }

        public SecureString GetPassword(string domain)
        {
            return Entries.Where(e => e.Domain == domain).Select(e => e.Password).First();
        }

        public void DeleteAllEntries()
        {
            Entries.Clear();

            _dbEntryRepository.DeleteDb();
        }

        public void SaveAllEntries()
        {
            _dbEntryRepository.WriteEntriesToDb(Entries);
        }
    }
}
