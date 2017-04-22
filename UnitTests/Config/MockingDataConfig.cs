using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRing.Interfaces;

namespace UnitTests.Config
{
    class MockingDataConfig : IDataConfig
    {

        public string dbPath { get; private set; }
        public string metaPath { get; private set; }
        private string configPath;

        public MockingDataConfig(string meta, string db, string config)
        {
            metaPath = meta;
            dbPath = db;
            configPath = config;
        }

        public int GetStorageCount()
        {
            return 0;
        }

        public void ClearConfig()
        {
            return;
        }

        public void UpdateConfig(int count)
        {
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
