namespace KRing.Interfaces
{
    public interface IDataConfig
    {
        string dbPath { get; }
        string metaPath { get; }
        int GetStorageCount();
        void ClearConfig();
        void UpdateConfig(int count);
    }
}