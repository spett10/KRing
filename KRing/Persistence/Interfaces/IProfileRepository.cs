using KRing.Core.Model;
using System.Threading.Tasks;

namespace KRing.Persistence.Interfaces
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