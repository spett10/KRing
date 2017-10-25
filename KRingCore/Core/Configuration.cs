using System;
using System.Configuration;

namespace KRingCore.Core
{
    public static class Configuration
    {
        public static int PBKDF2LoginIterations { get; private set; }
        public static int PBKDF2DeriveIterations { get; private set; }
        public static int OLD_PBKDF2DeriveIterations { get; private set; }
        public static int OLD_PBKDF2LoginIterations { get; private set; }
        public static bool TryOldValues { get; private set; }

        static Configuration()
        {
            Configuration.PBKDF2LoginIterations = Int32.Parse(ConfigurationManager.AppSettings["PBKDF2LoginIterations"]);
            Configuration.PBKDF2DeriveIterations = Int32.Parse(ConfigurationManager.AppSettings["PBKDF2DeriveIterations"]);
            Configuration.OLD_PBKDF2LoginIterations = Int32.Parse(ConfigurationManager.AppSettings["OLD_PBKDF2LoginIterations"]);
            Configuration.OLD_PBKDF2DeriveIterations = Int32.Parse(ConfigurationManager.AppSettings["OLD_PBKDF2DeriveIterations"]);

            var tryOldValues = Int32.Parse(ConfigurationManager.AppSettings["TryOldValues"]);
            if (tryOldValues == 1)
            {
                TryOldValues = true;
            }
            else
            {
                TryOldValues = false;
            }
        }
    }
}
