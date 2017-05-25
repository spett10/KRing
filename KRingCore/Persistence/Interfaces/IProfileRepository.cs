using KRingCore.Core.Model;
using System.Threading.Tasks;

namespace KRingCore.Persistence.Interfaces
{
    public interface IProfileRepository
    {
        void DeleteUser();
        Task DeleteUserAsync();
        User ReadUser();
        Task<User> ReadUserAsync();
        void WriteUser(User user);
        Task WriteUserAsync(User user);
    }
}