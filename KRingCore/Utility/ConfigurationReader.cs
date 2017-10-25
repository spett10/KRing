using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRingCore.Utility
{
    public static class ConfigurationReader
    {
        public static class AlgorithmParameters
        {
            public static readonly int oldPkeIteration = Int32.Parse(ConfigurationManager.AppSettings["oldPKERounds"]);
            public static readonly int newPkeIteration = Int32.Parse(ConfigurationManager.AppSettings["newPKERounds"]);
            public static readonly int keyDerivationRounds = Int32.Parse(ConfigurationManager.AppSettings["keyDerivationRounds"]);
        }
    }
}
