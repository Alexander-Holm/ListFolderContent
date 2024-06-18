using System.Windows;
using System.Windows.Controls;

namespace ListFolderContent.Controls
{
    public partial class FontSize : UserControl
    {
        public FontSize()
        {
            InitializeComponent();
        }

        public Classes.FontSize Value
        {
            get { return (Classes.FontSize)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(Classes.FontSize), typeof(FontSize), new PropertyMetadata((Classes.FontSize)0));
    }
}
