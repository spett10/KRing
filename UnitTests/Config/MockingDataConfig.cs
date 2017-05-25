using System;
using System.Threading.Tasks;
using KRingCore.Interfaces;

namespace UnitTests.Config
{
    class MockingDataConfig : IDataConfig
    {

        public string dbPath { get; private set; }
        public string metaPath { get; private set; }
        private string configPath;

        private int _count;

        public MockingDataConfig(string meta, string db, string config)
        {
            metaPath = meta;
            dbPath = db;
            configPath = config;

            _count = 0;
        }

        public int GetStorageCount()
        {
            return _count;
        }

        public void ClearConfig()
        {
            _count = 0;
            return;
        }

        public void UpdateConfig(int count)
        {
            _count = count;
            return;
        }

        public Task<int> GetStorageCountAsync()
        {
            throw new NotImplementedException();
        }

        public Task ClearConfigAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateConfigAsync(int count)
        {
            throw new NotImplementedException();
        }
    }
}
