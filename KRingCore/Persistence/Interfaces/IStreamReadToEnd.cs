using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRingCore.Persistence.Interfaces
{
    public interface IStreamReadToEnd
    {
        string ReadToEnd(string filename);
    }
}
