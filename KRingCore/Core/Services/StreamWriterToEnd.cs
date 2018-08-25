using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRingCore.Core.Services
{
    public class StreamWriterToEnd : IStreamWriterToEnd
    {
        public void WriteToNewFile(string filename, string data)
        {
            ValidateInput(filename, data);

            using (FileStream fileStream = new FileStream(filename, FileMode.Create))
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.Write(data);
            }
        }

        public async Task WriteToNewFileAsync(string filename, string data)
        {
            ValidateInput(filename, data);

            using (FileStream fileStream = new FileStream(filename, FileMode.Create))
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                await streamWriter.WriteAsync(data);
            }
        }

        private void ValidateInput(string filename, string data)
        {
            if (string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(data))
                throw new ArgumentException();
        }
    }
}
