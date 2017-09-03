using KRingCore.Persistence.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KRingCore.Persistence.Interfaces
{
    public interface IStoredPasswordReader
    {
        List<StoredPassword> LoadEntriesFromDb();
        bool DecryptionErrorOccured { get; }
        Task<List<StoredPassword>> LoadEntriesFromDbAsync();
    }
}
