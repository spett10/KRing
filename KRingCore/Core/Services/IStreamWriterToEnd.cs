using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRingCore.Core.Services
{
    public interface IStreamWriterToEnd
    {
        void WriteToNewFile(string filename, string data);
        Task WriteToNewFileAsync(string filename, string data);
    }
}
