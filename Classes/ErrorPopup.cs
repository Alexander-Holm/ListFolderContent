using System.Windows;

namespace ListFolderContent.Classes
{
    internal class ErrorPopup
    {
        public static void Show(string message)
        {
            string windowTitle = "Felmeddelande";
            MessageBox.Show(
                message, 
                windowTitle, 
                MessageBoxButton.OK,
                MessageBoxImage.Error,
                // Gör ingenting
                MessageBoxResult.OK,
                // Behövs för att visas framför Word-dokumentet som har fokus
                MessageBoxOptions.DefaultDesktopOnly
            );
        }
    }
}
