using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Data;

namespace ListFolderContent.Classes
{
    public class FontSizes
    {
        public FontSize Title { get; set; }
        public FontSize Subtitle { get; set; }
        public FontSize Letter { get; set; }
        public FontSize Filename { get; set; }
    }

    [JsonConverter(typeof(FontSizeJsonConverter))]
    public struct FontSize : INotifyDataErrorInfo
    {
        private static readonly int MAX_VALUE = 128;
        private static readonly int MIN_VALUE = 1;
        private readonly int _value;        

        // Sätter värde med int, exempel: FontSize fontSize = 10;
        public static implicit operator FontSize(int size) => new FontSize(size);
        // Läser värde som int, exempel: int currentSize = fontSize;
        public static implicit operator int(FontSize fontSize) => fontSize._value;
        public override string ToString() => _value.ToString();

        // Endast privat constructor.
        // Anropas när ett värde sätts;
        // hela objektet skapas på nytt varje gång man sätter ett värde!
        private FontSize(int value) 
        {
            if (MAX_VALUE >= value && value >= MIN_VALUE)
                ErrorMessage = "";
            else ErrorMessage = $"Måste vara mellan {MIN_VALUE} & {MAX_VALUE}";
            ErrorsChanged?.Invoke(this, new(null));

            _value = value;
        }

        public string ErrorMessage { get; set; }
        public bool HasErrors => !string.IsNullOrEmpty(ErrorMessage);        
        public IEnumerable GetErrors(string? propertyName)
        {
            return new string[] { ErrorMessage };
        }
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    }


    // För att skriva FontSize som en int i JSON
    // (inte ta med public properties så som ErrorMessage, HasErrors, m.m.)
    public class FontSizeJsonConverter : JsonConverter<FontSize>
    {
        public override FontSize Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();
            if (int.TryParse(value, out int number))
                return number;
            else return 0;
        }

        public override void Write(Utf8JsonWriter writer, FontSize value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }


    // För bindings i XAML
    [ValueConversion(typeof(FontSize), typeof(string))]
    public class FontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return value.ToString() ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string valueAsString = (string)value;
                FontSize fontSize = int.Parse(valueAsString);
                return fontSize;
            }
            catch { return DependencyProperty.UnsetValue; }
        }
    }
}
