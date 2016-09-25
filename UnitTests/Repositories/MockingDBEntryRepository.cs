using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using KRing.DTO;
using KRing.Persistence.Interfaces;
using KRing.Persistence.Model;

namespace UnitTests.Repositories
{
    class MockingDBEntryRepository : IDbEntryRepository
    {
        private List<DBEntry> _entries;
        public bool _addEntryCalled = false;
        public bool _updateEntryCalled = false;
        public bool _deleteEntryCalled = false;
        public bool _viewEntryCalled = false;

        public MockingDBEntryRepository()
        {
            _entries = new List<DBEntry>();
        }

        public int EntryCount
        {
            get
            {
                return _entries.Count;
            }
        }

        public void AddEntry(DbEntryDto newDto)
        {
            _addEntryCalled = true;
            _entries.Add(new DBEntry(newDto.Domain, newDto.Password));
        }

        public void DeleteAllEntries()
        {
            _entries.Clear();
        }

        public void DeleteEntry(int index)
        {
            _deleteEntryCalled = true;
        }

        public void DeleteEntry(string domain)
        {
            _deleteEntryCalled = true;
        }

        public bool ExistsEntry(string domain)
        {
            throw new NotImplementedException();
        }

        public List<DBEntry> GetEntries()
        {
            return _entries;
        }

        public DBEntry GetEntry(int index)
        {
            return _entries.ElementAt(index);
        }

        public SecureString GetPasswordFromCount(int count)
        {
            _viewEntryCalled = true;
            return _entries.ElementAt(count).Password;
        }

        public SecureString GetPasswordFromDomain(string domain)
        {
            throw new NotImplementedException();
        }

        public bool IsDbEmpty()
        {
            throw new NotImplementedException();
        }

        public List<DBEntry> LoadEntriesFromDb()
        {
            throw new NotImplementedException();
        }

        public void UpdateEntry(DbEntryDto updatedEntry)
        {
            _updateEntryCalled = true;
        }

        public void WriteEntriesToDb()
        {
            throw new NotImplementedException();
        }
    }
}
