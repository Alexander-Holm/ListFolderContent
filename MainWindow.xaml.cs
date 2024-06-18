using Word = Microsoft.Office.Interop.Word;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using ListFolderContent.Classes;
using ListFolderContent.Classes.Validation;

namespace ListFolderContent
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _folderPath;
        public string FolderPath { 
            get { return _folderPath; }
            set { _folderPath = value; NotifyPropertyChanged(); }
        }
        public string DocumentTitle { get; set; }
        public string DocumentSubtitle { get; set; }
        private List<Font>? _fontList;
        public List<Font>? FontList
        {
            get { return _fontList; }
            set { _fontList = value; NotifyPropertyChanged(); }
        }
        private string _selectedFont;
        public string SelectedFont
        {
            get { return _selectedFont; }
            set { _selectedFont = value; NotifyPropertyChanged(); }
        }
        public FontSizes FontSizes { get; set; }
        public bool WriteEmptyLetters { get; set; }
        public bool IncludeDirectories { get; set; }
        public int Columns { get; set; }
        public ValidationCommand SaveCommand { get; set; }
        public ValidationCommand SubmitCommand { get; set; }

        public MainWindow()
        {
            Config? config = null;
            // Config.Load() returnerar null om det inte finns någon config-fil
            try { config = Config.Load(); }
            // Behöver ingen catch,
            // new Config() både för exceptions eller när config är null.
            // new Config() har default-värden.
            finally { config ??= new Config(); }

            DocumentTitle = config.Title;
            DocumentSubtitle = config.Subtitle;
            FontSizes = config.FontSizes;
            WriteEmptyLetters = config.IncludeEmptyLetters;
            IncludeDirectories = config.IncludeDirectories;
            Columns = config.Columns;
            _folderPath = config.FolderPath;

            FontList = null;
            _selectedFont = config.Font;
            // Väntar inte på att fonts ska laddas.
            // Öppnar Word för att läsa fonts så det kan ta någon sekund.
            Task.Run(LoadFontList);

            SaveCommand = new(SaveConfig);
            SubmitCommand = new(Submit);

            InitializeComponent();
        }


        private void LoadFontList()
        {
            var wordApp = new Word.Application();
            var wordFonts = DataAccess.ReadFontsFromWord(wordApp).Order();
            wordApp.Quit();
            var installedFonts = DataAccess.InstalledFonts();

            var fontList = new List<Font>();
            foreach (var wordFont in wordFonts)
            {
                bool isInstalled = installedFonts.Any(installed => installed.Name == wordFont);
                fontList.Add(new Font(wordFont, isInstalled));
            }

            FontList = fontList;
        }

        
        private void Submit()
        {
            if (Directory.Exists(FolderPath) == false)
            {
                ErrorPopup.Show("Kan inte hitta den valda mappen");
                return;
            }
            IEnumerable<string>? content;
            try { 
                var files = DataAccess.ReadFiles(FolderPath);
                if (IncludeDirectories)
                {
                    var directories = DataAccess.ReadDirectories(FolderPath);
                    content = files.Concat(directories);
                }
                else content = files;
                if (content.Any() == false)
                {
                    ErrorPopup.Show("Den valda mappen saknar innehåll");
                    return;
                }
            }
            catch { ErrorPopup.Show("Kan inte läsa innehållet i den valda mappen"); return; }            

            try
            {
                var wordApp = new Word.Application();
                wordApp.Visible = true;
                wordApp.Activate(); // Focus

                var document = new WordDocument(wordApp, SelectedFont, FontSizes);
                document.AddTitle(DocumentTitle, DocumentSubtitle);
                document.AddList(content.Order(), Columns, WriteEmptyLetters);
            }
            catch
            {
                ErrorPopup.Show("Något gick fel när dokumentet skapades");
            }
        }

        private void SaveConfig()
        {
            try
            {
                Config config = new()
                {
                    Title = DocumentTitle,
                    Subtitle = DocumentSubtitle,
                    Font = SelectedFont,
                    FontSizes = FontSizes,
                    IncludeEmptyLetters = WriteEmptyLetters,
                    IncludeDirectories = IncludeDirectories,
                    Columns = Columns,
                    FolderPath = FolderPath
                };
                config.Save();
                MessageBox.Show("Inställningar sparade", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch { ErrorPopup.Show("Det gick inte att spara inställningarna"); }
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}