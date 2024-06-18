using System.Windows;
using System.Windows.Controls;
using ListFolderContent.Classes;

namespace ListFolderContent.Controls
{
    public partial class FontName : UserControl
    {       
        public FontName()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(FontName), new PropertyMetadata(""));


        public List<Font> FontList
        {
            get { return (List<Font>)GetValue(FontListProperty); }
            set { SetValue(FontListProperty, value); }
        }
        public static readonly DependencyProperty FontListProperty =
            DependencyProperty.Register("FontList", typeof(List<Font>), typeof(FontName), new PropertyMetadata(new PropertyChangedCallback(FindSelectedIndex)));

        // Körs när FontList sätts.
        // Behöver sätta SelectedIndex manuellt för att dropdown på ComboBox ska
        // vara nerscrollad till den font som står i textrutan.
        private static void FindSelectedIndex(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisControl = (FontName)d;
            var newFontList = (List<Font>)e.NewValue;
            thisControl.SelectedIndex = newFontList.FindIndex(font => font.Name == thisControl.Text);
        }

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(FontName), new PropertyMetadata(0));






    }
}
