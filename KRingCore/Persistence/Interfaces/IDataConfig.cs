using System.Threading.Tasks;

namespace KRingCore.Interfaces
{
    public interface IDataConfig
    {
        string dbPath { get; }
        int GetStorageCount();
        Task<int> GetStorageCountAsync();
        void ClearConfig();
        Task ClearConfigAsync();
        void UpdateConfig(int count);
        Task UpdateConfigAsync(int count);
    }
}