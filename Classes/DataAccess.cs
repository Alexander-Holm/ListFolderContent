using Word = Microsoft.Office.Interop.Word;
using System.IO;
using System.Drawing;
using System.Drawing.Text;

namespace ListFolderContent.Classes
{
    internal static class DataAccess
    {
        public static DirectoryInfo ExecutingDirectory()
        {
            // Borde kanske vara någon felhantering här,
            // vet inte när Environment.ProcessPath kan misslyckas.
            string binaryFile = Environment.ProcessPath!;
            // new DirectoryInfo ger path till filen,
            // parent är path till mappen den ligger i.
            DirectoryInfo directoryInfo = new DirectoryInfo(binaryFile).Parent!;
            return directoryInfo;
        }

        public static string? ParentDirectory()
        {
            var binaryFolder = ExecutingDirectory();
            if (binaryFolder.Parent != null)
                return binaryFolder.Parent.FullName;
            else return null;
        }

        public static FontFamily[] InstalledFonts()
        {
            return new InstalledFontCollection().Families;
        }

        public static List<string> ReadFontsFromWord(Word.Application wordApp)
        {
            var fontsEnumerator = wordApp.FontNames.GetEnumerator();
            List<string> fontList = new();
            while (fontsEnumerator.MoveNext())
            {
                string font = (string)fontsEnumerator.Current;
                fontList.Add(font);
            }
            return fontList;
        }

        public static List<string> ReadFiles(string path)
        {
            string[] filePaths = Directory.GetFiles(path);
            var fileNames = filePaths.Select(path => {
                string fileName = Path.GetFileNameWithoutExtension(path);
                // Vissa filer har inget namn, t.ex .gitignore.
                // Jag räknar extension som namn i det fallet.
                if(string.IsNullOrEmpty(fileName))
                    fileName = Path.GetExtension(path);
                return fileName;
            });
            return fileNames.ToList()!;
        }

        public static List<string> ReadDirectories(string path)
        {
            var fullPaths = Directory.GetDirectories(path);
            var directoryNames = new List<string>();
            foreach (string fullPath in fullPaths)
            {
                string nameOnly = new DirectoryInfo(fullPath).Name;
                directoryNames.Add(nameOnly);
            }
            return directoryNames;
        }
    }
}
