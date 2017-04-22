using KRing.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace KRing.Persistence
{
    public class DataConfig : IDataConfig
    {
        public string metaPath { get; private set; }
        public string dbPath { get; private set; } 
        public string configPath { get; private set; }

        public DataConfig(string meta, string db, string config)
        {
            metaPath = meta;
            dbPath = db;
            configPath = config;
        }

        public void ClearConfig()
        {
            FileUtil.FilePurge(configPath, "0");
        }

        public async Task ClearConfigAsync()
        {
            await FileUtil.FilePurgeAsync(configPath, "0");
        }

        public void UpdateConfig(int count)
        {
            using (StreamWriter configWriter = new StreamWriter(configPath))
            {
                configWriter.WriteLine(count);
            }
        }

        public async Task UpdateConfigAsync(int count)
        {
            using (StreamWriter configWriter = new StreamWriter(configPath))
            {
                await configWriter.WriteLineAsync(count.ToString());
            }
        }

        public async Task<int> GetStorageCountAsync()
        {
            int count = 0;

            using (StreamReader sr = new StreamReader(configPath))
            {
                var readCount = await sr.ReadLineAsync();
                if (readCount != null)
                {
                    int.TryParse(readCount, out count);
                }
                else
                {
                    count = 0;
                }
            }

            return count;
        }

        public int GetStorageCount()
        {
            int count = 0;

            using (StreamReader sr = new StreamReader(configPath))
            {
                var readCount = sr.ReadLine();
                if (readCount != null)
                {
                    int.TryParse(readCount, out count);
}
                else
                {
                    count = 0;
                }
            }

            return count;
        }
    }
    
}
