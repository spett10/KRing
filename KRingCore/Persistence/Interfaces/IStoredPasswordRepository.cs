﻿using System.Collections.Generic;
using System.Security;
using KRingCore.Persistence.Model;
using System.Threading.Tasks;

namespace KRingCore.Persistence.Interfaces
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
        string GetPasswordFromCount(int count);
        string GetPasswordFromDomain(string domain);
        bool IsDbEmpty();
        List<StoredPassword> LoadEntriesFromDb();
        Task<List<StoredPassword>> LoadEntriesFromDbAsync();
        void UpdateEntry(StoredPassword updatedEntry);
        void WriteEntriesToDb();
        Task WriteEntriesToDbAsync();
        bool DecryptionErrorOccured { get; }
        bool EncryptionErrorOccured { get; }
    }
}