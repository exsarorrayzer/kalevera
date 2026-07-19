using System.Windows;
using System.Windows.Input;
using Kalevera.Models;
using Kalevera.ViewModels;

namespace Kalevera.Views;

public partial class MainWindow : Window
{
    private MainViewModel ViewModel => (MainViewModel)DataContext;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.LoadData();
        RequestBuilderCtrl.SendClicked += OnSendClicked;
        RequestBuilderCtrl.NewRequestClicked += OnNewRequestClicked;
        ViewModel.SettingsVM.ThemeChanged += OnThemeChanged;
        ApplyTheme(ViewModel.SettingsVM.ActiveTheme);
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        ViewModel.SaveData();
    }

    private void OnSendClicked(object? sender, EventArgs e)
    {
        if (ViewModel.SendRequestCommand.CanExecute(null))
            ViewModel.SendRequestCommand.Execute(null);
    }

    private void OnNewRequestClicked(object? sender, EventArgs e)
    {
        if (ViewModel.NewRequestCommand.CanExecute(null))
            ViewModel.NewRequestCommand.Execute(null);
    }

    private void ImportFetch_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.OpenFetchConverterCommand.CanExecute(null))
            ViewModel.OpenFetchConverterCommand.Execute(null);
    }

    private void SaveRequest_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SaveCurrentRequestCommand.CanExecute(null))
            ViewModel.SaveCurrentRequestCommand.Execute(null);
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SettingsVM.OpenSettingsCommand.CanExecute(null))
            ViewModel.SettingsVM.OpenSettingsCommand.Execute(null);
    }

    private void OnThemeChanged(object? sender, string themeName)
    {
        ApplyTheme(themeName);
    }

    private void ApplyTheme(string themeName)
    {
        var app = (App)Application.Current;
        var dict = app.Resources.MergedDictionaries;

        var themeFile = themeName switch
        {
            "Dark Blue" => "Themes/DarkBlueTheme.xaml",
            "Dark Purple" => "Themes/DarkPurpleTheme.xaml",
            "Dark Green" => "Themes/DarkGreenTheme.xaml",
            "OLED Black" => "Themes/OledBlackTheme.xaml",
            _ => "Themes/DarkRedTheme.xaml"
        };

        for (int i = dict.Count - 1; i >= 0; i--)
        {
            if (dict[i] is ResourceDictionary rd && rd.Source != null && rd.Source.ToString().Contains("Theme"))
            {
                dict.RemoveAt(i);
            }
        }

        dict.Add(new ResourceDictionary
        {
            Source = new Uri($"pack://application:,,,/{themeFile}", UriKind.Absolute)
        });
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        else
            DragMove();
    }

    private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
    private void Maximize_Click(object sender, RoutedEventArgs e) =>
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
