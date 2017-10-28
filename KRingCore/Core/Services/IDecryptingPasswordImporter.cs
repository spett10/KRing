using KRingCore.Persistence.Model;
using KRingCore.Persistence.Interfaces;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;

namespace KRingCore.Core.Services
{
    public interface IDecryptingPasswordImporter
    {
        List<StoredPassword> ImportPasswords(string filename, SecureString password, IStreamReadToEnd streamReader);
        Task<List<StoredPassword>> ImportPasswordsAsync(string filename, SecureString password);
    }
}
