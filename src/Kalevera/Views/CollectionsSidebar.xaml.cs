using System.Windows;
using System.Windows.Controls;
using Kalevera.Models;
using Kalevera.ViewModels;

namespace Kalevera.Views;

public partial class CollectionsSidebar : UserControl
{
    private MainViewModel MainVm => (MainViewModel)DataContext;

    public CollectionsSidebar()
    {
        InitializeComponent();
    }

    private void HistoryToggle_Click(object sender, RoutedEventArgs e)
    {
        if (MainVm.ToggleHistoryCommand.CanExecute(null))
            MainVm.ToggleHistoryCommand.Execute(null);
    }

    private void SaveCurrent_Click(object sender, RoutedEventArgs e)
    {
        if (MainVm.SaveCurrentRequestCommand.CanExecute(null))
            MainVm.SaveCurrentRequestCommand.Execute(null);
    }

    private void AddNewEmpty_Click(object sender, RoutedEventArgs e)
    {
        var req = new HttpRequestModel { Name = $"Request {MainVm.SavedRequestsVM.SavedRequests.Count + 1}" };
        MainVm.SavedRequestsVM.SavedRequests.Add(req);
        MainVm.SaveData();
    }

    private void OverwriteSavedRequest_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menu) return;
        if (menu.Tag is not HttpRequestModel target) return;

        var current = MainVm.RequestVM.Request;
        target.Name = current.Name;
        target.Method = current.Method;
        target.Url = current.Url;
        target.Headers = current.Headers.Select(h => new RequestHeader { Key = h.Key, Value = h.Value, IsEnabled = h.IsEnabled }).ToList();
        target.Parameters = current.Parameters.Select(p => new RequestParameter { Key = p.Key, Value = p.Value, IsEnabled = p.IsEnabled }).ToList();
        target.BodyType = current.BodyType;
        target.BodyContent = current.BodyContent;
        target.AuthType = current.AuthType;
        target.AuthUsername = current.AuthUsername;
        target.AuthPassword = current.AuthPassword;
        target.AuthToken = current.AuthToken;

        MainVm.SaveData();
    }

    private void DeleteSavedRequest_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menu) return;
        if (menu.Tag is not HttpRequestModel target) return;

        var result = MessageBox.Show(
            $"Delete \"{target.Name}\"?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            MainVm.SavedRequestsVM.SavedRequests.Remove(target);
            MainVm.SaveData();
        }
    }
}
