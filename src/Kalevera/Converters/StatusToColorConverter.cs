using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Kalevera.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int statusCode)
        {
            return statusCode switch
            {
                >= 200 and < 300 => new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)),
                >= 300 and < 400 => new SolidColorBrush(Color.FromRgb(0xFF, 0xC1, 0x07)),
                >= 400 and < 500 => new SolidColorBrush(Color.FromRgb(0xE6, 0x39, 0x46)),
                >= 500           => new SolidColorBrush(Color.FromRgb(0xFF, 0x4D, 0x5A)),
                _                => new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99)),
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
