using System.Globalization;
using System.Windows.Controls;

namespace ListFolderContent.Classes.Validation
{
    public class IntegerValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (int.TryParse(value as string, out int _))
                return ValidationResult.ValidResult;
            else return new ValidationResult(false, "Måste vara ett heltal");
        }
    }
}
