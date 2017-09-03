using KRingCore.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace KRingCore.Persistence
{
    public class DataConfig : IDataConfig
    {
        public string dbPath { get; private set; }

        public DataConfig(string db)
        {
            dbPath = db;
        }
    }
    
}
