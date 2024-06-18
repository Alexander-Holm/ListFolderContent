using Word = Microsoft.Office.Interop.Word;
using System.Reflection;

namespace ListFolderContent.Classes
{
    internal class WordDocument
    {
        private Word.Document document;

        private string _fontName = "";
        public string FontName 
        { 
            get { return _fontName; }
            set
            {
                if(value == _fontName) return;
                _fontName = value;
                document.Content.Font.Name = value;
            }
        }
        public FontSizes FontSizes { get; set; }

        // Constructor
        public WordDocument(Word.Application wordApp, string fontName, FontSizes fontSizes)
        {
            document = wordApp.Documents.Add();
            FontName = fontName;
            FontSizes = fontSizes;
        }

        #region Public methods
        public void AddTitle(string title, string subtitle)
        {
            Word.Range range;
            var centerAlign = Word.WdParagraphAlignment.wdAlignParagraphCenter;

            var titleParagraph = document.Paragraphs.Add();
            range = titleParagraph.Range;
            range.InsertBefore(title);
            range.Font.Size = FontSizes.Title;
            range.ParagraphFormat.Alignment = centerAlign;
            titleParagraph.SpaceAfter = 0;

            var subtitleParagraph = document.Paragraphs.Add();
            range = subtitleParagraph.Range;
            range.InsertBefore(subtitle);
            range.Font.Size = FontSizes.Subtitle;
            range.ParagraphFormat.Alignment = centerAlign;
            subtitleParagraph.SpaceAfter = 50;
        }

        public void AddList(IOrderedEnumerable<string> list, int columns, bool includeEmptyLetters)
        {
            var dictionary = CreateDictionary(list, includeEmptyLetters);
            var section = CreateSection(columns);
            var table = CreateTable(section.Range, dictionary.Count);

            for (int rowIndex = 1; rowIndex <= dictionary.Count; rowIndex++)
            {
                var row = table.Rows[rowIndex];
                var rowContent = dictionary.ElementAt(rowIndex - 1);
                char letter = rowContent.Key;
                List<string> files = rowContent.Value;

                FillRow(row, letter, files);
            }
        }
        #endregion

        #region Private methods
        protected Word.Section CreateSection(int columns)
        {
            var sectionType = Word.WdSectionStart.wdSectionContinuous;
            var section = document.Sections.Add(Missing.Value, sectionType);
            section.PageSetup.TextColumns.SetCount(columns);
            return section;
        }

        protected Word.Table CreateTable(Word.Range range, int rows)
        {
            int columns = 2;
            var autoFitEnabled = Word.WdDefaultTableBehavior.wdWord9TableBehavior;
            var fitContent = Word.WdAutoFitBehavior.wdAutoFitWindow;
            var table = document.Tables.Add(range, rows, columns, autoFitEnabled, fitContent);

            table.Borders.Enable = 0;
            table.Rows.Alignment = Word.WdRowAlignment.wdAlignRowLeft;
            table.Rows.AllowBreakAcrossPages = 0;
            // BottomPadding för alla celler, inte cm          
            table.BottomPadding = 20;

            var letterColumn = table.Columns[1];
            letterColumn.PreferredWidthType = Word.WdPreferredWidthType.wdPreferredWidthPercent;
            letterColumn.PreferredWidth = 0;

            var itemsColumn = table.Columns[2];
            itemsColumn.PreferredWidthType = Word.WdPreferredWidthType.wdPreferredWidthPercent;
            itemsColumn.PreferredWidth = 100;

            return table;
        }

        protected void FillRow(Word.Row row, char letter, List<string> files)
        {
            // Textstorlek
            var letterColumn = row.Cells[1].Range;            
            letterColumn.Font.Size = FontSizes.Letter;
            var filesColumn = row.Cells[2].Range;            
            filesColumn.Font.Size = FontSizes.Filename;

            // Padding
            // Kan inte använda PaddingTop på bara en kolumn,
            // båda kolumnerna får samma PaddingTop
            // även om den bara sätts på en av dem.
            // Sätter SpaceBefore på texten i andra kolumnen istället.
            var filesText = filesColumn.Paragraphs;                 
            filesText.LeftIndent = 8;
            // Första filnamnet läggs i samma höjd som bokstavsrubriken
            // (underdelen av texten ligger på samma linje),
            // oavsett textstorlek i kolumnerna
            filesText.SpaceBefore = Math.Max(1, FontSizes.Letter - FontSizes.Filename);

            // Alla filnamn som en string, inte separata paragrafer,
            // pga paragrafer har SpaceBefore, se ovan.
            string filesString = "";
            if (files.Count > 0)
            {
                int lastFileIndex = files.Count - 1;
                for (int i = 0; i < lastFileIndex; i++)
                {
                    // \v ger ny rad utan ny paragraf
                    // \n skapar en ny paragraf som förstör style
                    filesString += files[i] + "\v";
                }
                filesString += files[lastFileIndex];
            }

            letterColumn.Text = letter.ToString();
            filesColumn.Text = filesString;
        }

        protected Dictionary<char, List<string>> CreateDictionary(IOrderedEnumerable<string> fileNames, bool includeEmptyLetters)
        {
            // Kan inte vara SortedDictionary,
            // Å och Ä hamnar i fel ordning.
            var dictionary = new Dictionary<char, List<string>>();

            if (includeEmptyLetters)
            {
                for (char letter = 'A'; letter <= 'Z'; letter++)
                {
                    dictionary.Add(letter, []);
                }
                char[] arr = ['Å', 'Ä', 'Ö'];
                foreach (char letter in arr)
                {
                    dictionary.Add(letter, []);
                }
            }

            // Lägg in värden i dictionary
            foreach (string fileName in fileNames)
            {
                char letter = fileName[0];
                letter = char.ToUpper(letter);
                if (dictionary.TryGetValue(letter, out List<string>? letterFileNames))
                    letterFileNames.Add(fileName);
                else dictionary.Add(letter, [fileName]);
            }

            return dictionary;
        }
        #endregion
    }
}
