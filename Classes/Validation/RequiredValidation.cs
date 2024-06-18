using System.Globalization;
using System.Windows.Controls;

namespace ListFolderContent.Classes.Validation
{
    class RequiredValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string valueAsString = (string)value;
            if (string.IsNullOrEmpty(valueAsString))
                return new ValidationResult(false, "Kan inte lämnas tom");
            else return ValidationResult.ValidResult;
        }
    }
}
