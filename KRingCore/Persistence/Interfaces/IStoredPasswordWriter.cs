using System.Collections.Generic;
using System.Threading.Tasks;
using KRingCore.Persistence.Model;

namespace KRingCore.Persistence.Interfaces
{
    public interface IStoredPasswordWriter
    {
        Task WriteEntriesToDbAsync(List<StoredPassword> list);
        bool EncryptionErrorOccured { get; }
        void WriteEntriesToDb(List<StoredPassword> list);
    }
}
