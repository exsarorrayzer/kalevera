using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Kalevera.Models;

namespace Kalevera.Views;

public partial class RequestBuilder : UserControl
{
    public event EventHandler? SendClicked;
    public event EventHandler? NewRequestClicked;

    public RequestBuilder()
    {
        InitializeComponent();
    }

    private void Send_Click(object sender, RoutedEventArgs e)
    {
        SendClicked?.Invoke(this, EventArgs.Empty);
    }

    private void NewRequest_Click(object sender, RoutedEventArgs e)
    {
        NewRequestClicked?.Invoke(this, EventArgs.Empty);
    }
}

public class BodyTypeConverter : IValueConverter
{
    public static readonly BodyTypeConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BodyType current && parameter is string param)
        {
            return param switch
            {
                "None" => current == BodyType.None,
                "Json" => current == BodyType.Json,
                "Form" => current == BodyType.Form,
                "Raw" => current == BodyType.Raw,
                _ => false
            };
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isChecked && isChecked && parameter is string param)
        {
            return param switch
            {
                "None" => BodyType.None,
                "Json" => BodyType.Json,
                "Form" => BodyType.Form,
                "Raw" => BodyType.Raw,
                _ => BodyType.None
            };
        }
        return DependencyProperty.UnsetValue;
    }
}

public class AuthTypeVisibilityConverter : IValueConverter
{
    public static readonly AuthTypeVisibilityConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is AuthType current && parameter is string param)
        {
            var match = param switch
            {
                "None" => current == AuthType.None,
                "Basic" => current == AuthType.Basic,
                "Bearer" => current == AuthType.Bearer,
                _ => false
            };
            return match ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
