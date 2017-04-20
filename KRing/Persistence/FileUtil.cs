using System.IO;
using System.Threading.Tasks;

namespace KRing.Persistence
{
    public static class FileUtil
    {
        public static void LineChanger(string newText, string filePath, int lineToEdit)
        {
            string[] allLines = File.ReadAllLines(filePath);
            allLines[allLines.Length - 1] = newText;
            File.WriteAllLines(filePath, allLines);
        }

        public static void FilePurge(string filepath, string toWrite)
        {
            using (FileStream fileStream = new FileStream(filepath, FileMode.Create))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine(toWrite);
                }
            }
        }

        public async static Task FilePurgeAsync(string filepath, string toWrite)
        {
            using (FileStream fileStream = new FileStream(filepath, FileMode.Create))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    await streamWriter.WriteLineAsync(toWrite);
                }
            }
        }
    }
}
