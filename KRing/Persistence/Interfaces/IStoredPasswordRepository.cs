using System.Collections.Generic;
using System.Security;
using KRing.Persistence.Model;

namespace KRing.Persistence.Interfaces
{
    public interface IStoredPasswordRepository
    {
        int EntryCount { get; }

        void AddEntry(StoredPassword newDbEntry);
        void DeleteAllEntries();
        void DeleteEntry(string domain);
        void DeleteEntry(int index);
        bool ExistsEntry(string domain);
        List<StoredPassword> GetEntries();
        StoredPassword GetEntry(int index);
        SecureString GetPasswordFromCount(int count);
        SecureString GetPasswordFromDomain(string domain);
        bool IsDbEmpty();
        List<StoredPassword> LoadEntriesFromDb();
        void UpdateEntry(StoredPassword updatedEntry);
        void WriteEntriesToDb();
        bool DecryptionErrorOccured { get; }
        bool EncryptionErrorOccured { get; }
    }
}