using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ListFolderContent.Classes
{
    internal class Config
    {
        protected static string path = DataAccess.ExecutingDirectory().FullName + "\\inställningar.json";

        public string Title { get; set; } = "A-Ö";
        public string Subtitle { get; set; } = "Innehållsförteckning";
        public string Font { get; set; } = "Calibri";
        public FontSizes FontSizes { get; set; } = new()
        {
            Title = 28,
            Subtitle = 22,
            Letter = 18,
            Filename = 14
        };
		public bool IncludeEmptyLetters { get; set; } = true;
        public bool IncludeDirectories { get; set; } = true;
        public int Columns { get; set; } = 1;
        public string FolderPath { get; set; } = DataAccess.ParentDirectory() ?? DataAccess.ExecutingDirectory().FullName;


        public static Config? Load()
        {
            if (File.Exists(path) == false)
                return null;

            using StreamReader reader = new(path);
            string json = reader.ReadToEnd();
            return JsonSerializer.Deserialize<Config?>(json);
        }

        public void Save()
        {
            using StreamWriter writer = new(path);
            string json = JsonSerializer.Serialize(this);
            writer.Write(json);            
        }
    }
}
