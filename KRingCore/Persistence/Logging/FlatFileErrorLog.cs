using KRingCore.Core;
using KRingCore.Core.Model;
using KRingCore.Krypto;
using KRingCore.Krypto.Model;
using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace KRingCore.Persistence.Logging
{
    public class FlatFileErrorLog : ReleasePathDependent
    {
        private readonly string _logfile;
        private readonly string _logIntegrityFile;

        private readonly int _deriveIterations;

        public FlatFileErrorLog()
        {
#if DEBUG
            _logfile = ConfigurationManager.AppSettings["relativeLogPathDebug"];
            _logIntegrityFile = ConfigurationManager.AppSettings["relativeLogIntegrityPathDebug"];
#else
            _logfile = base.ReleasePathPrefix() + ConfigurationManager.AppSettings["relativeLogPath"];
            _logIntegrityFile = base.ReleasePathPrefix() + ConfigurationManager.AppSettings["relativeLogIntegrityPath"];
#endif

            _deriveIterations = Core.Configuration.PBKDF2DeriveIterations;
        }

        public FlatFileErrorLog(int deriveIterations)
        {
#if DEBUG
            _logfile = ConfigurationManager.AppSettings["relativeLogPathDebug"];
            _logIntegrityFile = ConfigurationManager.AppSettings["relativeLogIntegrityPathDebug"];
#else
            _logfile = base.ReleasePathPrefix() + ConfigurationManager.AppSettings["relativeLogPath"];
            _logIntegrityFile = base.ReleasePathPrefix() + ConfigurationManager.AppSettings["relativeLogIntegrityPath"];
#endif

            _deriveIterations = deriveIterations > 0 ? deriveIterations : throw new ArgumentOutOfRangeException(nameof(deriveIterations)); // TODO use uint instead
        }

        public void Log(string context, string message)
        {
            using (FileStream fs = new FileStream(_logfile, FileMode.Append))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                var now = DateTime.Now.ToString();

                sw.WriteLine(now + " [" + context + "] " + message);
            }
        }

        public void ClearLog()
        {
            File.WriteAllText(_logfile, string.Empty);
            File.WriteAllText(_logIntegrityFile, string.Empty);
        }

        public void AuthenticateLog(User user)
        {
            var bytes = File.ReadAllBytes(_logfile);
            var salt = CryptoHashing.GenerateSalt(64);

            using (SymmetricKey key = new SymmetricKey(user.Password, salt, _deriveIterations))
            {
                var macBytes = CryptoHashing.HMACSHA256(bytes, key.Bytes);

                var saltBase64 = Convert.ToBase64String(salt);
                var macBase64 = Convert.ToBase64String(macBytes);

                File.WriteAllLines(_logIntegrityFile, new string[] { saltBase64, macBase64 });
            }            
        }

        public bool CheckLogIntegrity(User user)
        {
            var bytes = File.ReadAllBytes(_logfile);

            var storedLines = File.ReadAllLines(_logIntegrityFile);
            var storedSalt = Convert.FromBase64String(storedLines.First());
            var storedMac = Convert.FromBase64String(storedLines.Skip(1).First());

            using (SymmetricKey key = new SymmetricKey(user.Password, storedSalt, _deriveIterations))
            {
                var mac = CryptoHashing.HMACSHA256(bytes, key.Bytes);

                return CryptoHashing.CompareByteArraysNoTimeLeak(mac, storedMac);
            }
        }
    }
}
