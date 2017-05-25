using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRingCore.Core
{
    public abstract class ReleasePathDependent
    {
        public string ReleasePathPrefix()
        {
            return Environment.CurrentDirectory;
        }
    }
}
