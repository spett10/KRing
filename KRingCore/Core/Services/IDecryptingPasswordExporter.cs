using KRingCore.Persistence.Model;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;

namespace KRingCore.Core.Services
{
    public interface IDecryptingPasswordExporter
    {
        List<StoredPassword> ImportPasswords(string filename, SecureString password);
        Task<List<StoredPassword>> ImportPasswordsAsync(string filename, SecureString password);
    }
}
