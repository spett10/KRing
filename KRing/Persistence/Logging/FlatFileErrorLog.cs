﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRing.Persistence.Logging
{
    public class FlatFileErrorLog
    {
        private string _logfile;

        public FlatFileErrorLog()
        {
#if DEBUG
            _logfile = ConfigurationManager.AppSettings["relativeLogPathDebug"];
#else
            _logfile = Environment.CurrentDirectory + ConfigurationManager.AppSettings["relativeLogPath"];
#endif

            /* Clear log on each startup, else it will grow forever */
            FileUtil.FilePurge(_logfile, "");
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
    }
}
