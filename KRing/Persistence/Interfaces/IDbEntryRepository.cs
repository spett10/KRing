using System.Collections.Generic;
using System.Security;
using KRing.DTO;
using KRing.Persistence.Model;

namespace KRing.Persistence.Interfaces
{
    public interface IDbEntryRepository
    {
        int EntryCount { get; }

        void AddEntry(DbEntryDto newDto);
        void DeleteAllEntries();
        void DeleteEntry(string domain);
        void DeleteEntry(int index);
        bool ExistsEntry(string domain);
        List<DBEntry> GetEntries();
        DBEntry GetEntry(int index);
        SecureString GetPasswordFromCount(int count);
        SecureString GetPasswordFromDomain(string domain);
        bool IsDbEmpty();
        List<DBEntry> LoadEntriesFromDb();
        void UpdateEntry(DbEntryDto updatedEntry);
        void WriteEntriesToDb();
    }
}