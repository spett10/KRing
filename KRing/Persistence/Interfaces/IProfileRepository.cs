using KRing.Core.Model;

namespace KRing.Persistence.Interfaces
{
    public interface IProfileRepository
    {
        void DeleteUser();
        User ReadUser();
        void WriteUser(User user);
    }
}