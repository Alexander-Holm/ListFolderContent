using System.Windows;
using System.Windows.Input;

namespace ListFolderContent.Classes.Validation
{
    public class ValidationCommand : ICommand
    {
        private Action _execute;
        public ValidationCommand(Action execute)
        {
            _execute = execute;
        }

        public void Execute(object? parameter)
        {
            _execute.Invoke();
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter is DependencyObject node)
                return AreAllChildrenValid(node);
            else return false;
        }
        // https://stackoverflow.com/a/4650392
        private bool AreAllChildrenValid(DependencyObject node)
        {
            if (System.Windows.Controls.Validation.GetHasError(node))
                return false;

            return LogicalTreeHelper.GetChildren(node)
                .OfType<DependencyObject>()
                .All(AreAllChildrenValid);
        }

        // Låter WPF automatiskt sköta när Commands kör CanExecute
        // Vet inte hur tungt det är att kolla igenom children med LogicalTreeHelper.
        // Kan skapa en funktion NotifyCanExecuteChanged i den här klassen,
        // som kan anropas manuellt från de setters som påverkar CanExecute.
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
