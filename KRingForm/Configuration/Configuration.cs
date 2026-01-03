namespace KRingForm.Configuration
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
            // Iteration count set as suggested as of 2025 
            Configuration.PBKDF2LoginIterations = 600000;
            Configuration.PBKDF2DeriveIterations = 600000;
            Configuration.OLD_PBKDF2LoginIterations = 500000;
            Configuration.OLD_PBKDF2DeriveIterations = 500000;
            Configuration.ExportImportIterations = 600000;
            Configuration.TryOldValues = true;
        }
    }
}
