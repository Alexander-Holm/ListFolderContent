using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ListFolderContent.Classes;

namespace ListFolderContent.Controls
{
    /// <summary>
    /// Interaction logic for FolderPicker.xaml
    /// </summary>
    public partial class FolderPicker : UserControl
    {
        public FolderPicker()
        {
            InitializeComponent();
        }
        private void OpenFolderPicker(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog();
            folderDialog.InitialDirectory = Directory.Exists(FolderPath)
                ? FolderPath
                : DataAccess.ExecutingDirectory().FullName;

            bool? folderSelected = folderDialog.ShowDialog();
            if (folderSelected == true)
                FolderPath = folderDialog.FolderName;
        }

        public string FolderPath
        {
            get { return (string)GetValue(FolderPathProperty); }
            set { SetValue(FolderPathProperty, value); }
        }
        // Using a DependencyProperty as the backing store for FolderPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FolderPathProperty =
            DependencyProperty.Register("FolderPath", typeof(string), typeof(FolderPicker), new PropertyMetadata(""));
    }
}
