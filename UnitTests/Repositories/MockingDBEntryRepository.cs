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
    class MockingDBEntryRepository : IStoredPasswordRepository
    {
        private List<StoredPassword> _entries;
        public bool _addEntryCalled = false;
        public bool _updateEntryCalled = false;
        public bool _deleteEntryCalled = false;
        public bool _viewEntryCalled = false;
        public bool _loadCalled = false;
        public bool _saveCalled = false;
        public bool _deleteAllCalled = false;

        public MockingDBEntryRepository()
        {
            _entries = new List<StoredPassword>();
        }

        public int EntryCount
        {
            get
            {
                return _entries.Count;
            }
        }

        public bool DecryptionErrorOccured
        {
            get
            {
                return false;
            }
        }

        public bool EncryptionErrorOccured
        {
            get
            {
                return false;
            }
        }

        public void AddEntry(StoredPassword newDto)
        {
            _addEntryCalled = true;
            _entries.Add(new StoredPassword(newDto.Domain, newDto.PlaintextPassword));
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

        public List<StoredPassword> GetEntries()
        {
            return _entries;
        }

        public StoredPassword GetEntry(int index)
        {
            return _entries.ElementAt(index);
        }

        public string GetPasswordFromCount(int count)
        {
            _viewEntryCalled = true;
            return _entries.ElementAt(count).PlaintextPassword;
        }

        public string GetPasswordFromDomain(string domain)
        {
            throw new NotImplementedException();
        }

        public bool IsDbEmpty()
        {
            return !(_entries.Count > 0);
        }

        public List<StoredPassword> LoadEntriesFromDb()
        {
            _loadCalled = true;
            return _entries;
        }

        public void UpdateEntry(StoredPassword updatedEntry)
        {
            _updateEntryCalled = true;
        }

        public void WriteEntriesToDb()
        {
            _saveCalled = true;
            return;
        }

        public Task<List<StoredPassword>> LoadEntriesFromDbAsync()
        {
            throw new NotImplementedException();
        }

        public Task WriteEntriesToDbAsync()
        {
            throw new NotImplementedException();
        }
    }
}
