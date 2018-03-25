using KRingCore.Properties;
using System;
using System.Configuration;
using System.Resources;

namespace KRingCore.Core
{
    public static class Configuration
    {
        public static int PBKDF2LoginIterations { get; private set; }
        public static int PBKDF2DeriveIterations { get; private set; }
        public static int OLD_PBKDF2DeriveIterations { get; private set; }
        public static int OLD_PBKDF2LoginIterations { get; private set; }
        public static bool TryOldValues { get; private set; }
        public static int ExportImportIterations { get; private set; }

        static Configuration()
        {
            Configuration.PBKDF2LoginIterations = 500000;
            Configuration.PBKDF2DeriveIterations = 500000;
            Configuration.OLD_PBKDF2LoginIterations = 10000;
            Configuration.OLD_PBKDF2DeriveIterations = 10000;
            Configuration.ExportImportIterations = 500000;
            Configuration.TryOldValues = true;
        }
    }
}
