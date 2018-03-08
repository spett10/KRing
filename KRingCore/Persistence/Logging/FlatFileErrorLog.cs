using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using KRingCore.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRingCore.Core.Model;
using System.Security.Cryptography;
using KRingCore.Core.Services;
using KRingCore.Security;

namespace KRingCore.Persistence.Logging
{
    public class FlatFileErrorLog : ReleasePathDependent
    {
        private readonly string _logfile;
        private readonly string _logIntegrityFile;

        public FlatFileErrorLog()
        {
#if DEBUG
            _logfile = ConfigurationManager.AppSettings["relativeLogPathDebug"];
            _logIntegrityFile = ConfigurationManager.AppSettings["relativeLogIntegrityPathDebug"];
#else
            _logfile = base.ReleasePathPrefix() + ConfigurationManager.AppSettings["relativeLogPath"];
            _logIntegrityFile = ConfigurationManager.AppSettings["relativeLogIntegrityPath"];
#endif

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

            using (SymmetricKey key = new SymmetricKey(user.Password, user.SecurityData.MacKeySalt))
            {
                var macBytes = CryptoHashing.HMACSHA256(bytes, key.Bytes);

                File.WriteAllBytes(_logIntegrityFile, macBytes);
            }            
        }

        public bool CheckLogIntegrity(User user)
        {
            var bytes = File.ReadAllBytes(_logfile);

            var storedMac = File.ReadAllBytes(_logIntegrityFile);

            using (SymmetricKey key = new SymmetricKey(user.Password, user.SecurityData.MacKeySalt))
            {
                var mac = CryptoHashing.HMACSHA256(bytes, key.Bytes);
                
                return CryptoHashing.CompareByteArraysNoTimeLeak(mac, storedMac);
            }
        }
    }
}
