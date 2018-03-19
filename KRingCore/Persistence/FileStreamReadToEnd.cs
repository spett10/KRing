using KRingCore.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRingCore.Persistence
{
    public class FileStreamReadToEnd : IStreamReadToEnd
    {
        public string ReadToEnd(string filename)
        {
            ValidateInput(filename);

            using (FileStream fileStream = new FileStream(filename, FileMode.Open))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                return streamReader.ReadToEnd();
            }
        }

        public Task<string> ReadToEndAsync(string filename)
        {
            ValidateInput(filename);

            using (FileStream fileStream = new FileStream(filename, FileMode.Open))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                return streamReader.ReadToEndAsync();
            }
        }

        private void ValidateInput(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentException();
            }
        }
    }
}
