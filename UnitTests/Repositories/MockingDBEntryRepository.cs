using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
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
        public bool _loadCalled = false;
        public bool _saveCalled = false;
        public bool _deleteAllCalled = false;

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

        public void AddEntry(DBEntry newDto)
        {
            _addEntryCalled = true;
            _entries.Add(new DBEntry(newDto.Domain, newDto.Password));
        }

        public void DeleteAllEntries()
        {
            _deleteAllCalled = true;
            _entries.Clear();
        }

        public void DeleteEntry(int index)
        {
            _deleteEntryCalled = true;
            var toDelete = _entries.ElementAt(index);
            _entries.Remove(toDelete);
        }

        public void DeleteEntry(string domain)
        {
            _deleteEntryCalled = true;
            var toDelete = _entries.SingleOrDefault(e => e.Domain == domain);
            if (toDelete != null) _entries.Remove(toDelete);
        }

        public bool ExistsEntry(string domain)
        {
            return _entries.Exists(e => e.Domain == domain);
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
            return !(_entries.Count > 0);
        }

        public List<DBEntry> LoadEntriesFromDb()
        {
            _loadCalled = true;
            return _entries;
        }

        public void UpdateEntry(DBEntry updatedEntry)
        {
            _updateEntryCalled = true;
        }

        public void WriteEntriesToDb()
        {
            _saveCalled = true;
            return;
        }
    }
}
