namespace KRing.Persistence
{
    public class DataConfig
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
    }
    
}
