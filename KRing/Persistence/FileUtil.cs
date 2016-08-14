using System.IO;

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
    }
}
