using KRingCore.Persistence.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KRingCore.Core.Services
{
    public interface IPasswordImporter
    {
        List<StoredPassword> ImportPasswords(string filename);
        Task<List<StoredPassword>> ImportPasswordsAsync(string filename);
    }
}
