using KRingCore.Core.Model;
using KRingCore.Persistence.Model;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;

namespace KRingCore.Core.Services
{
    public interface IEncryptingPasswordExporter
    {
        string ExportPasswords(List<StoredPassword> passwords);
        Task<string> ExportPasswordsAsync(List<StoredPassword> passwords);
    }
}
