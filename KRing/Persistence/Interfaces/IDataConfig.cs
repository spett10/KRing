using System.Threading.Tasks;

namespace KRing.Interfaces
{
    public interface IDataConfig
    {
        string dbPath { get; }
        string metaPath { get; }
        int GetStorageCount();
        Task<int> GetStorageCountAsync();
        void ClearConfig();
        Task ClearConfigAsync();
        void UpdateConfig(int count);
        Task UpdateConfigAsync(int count);
    }
}