using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kalevera.Models;

namespace Kalevera.ViewModels;

public partial class SavedRequestsViewModel : ObservableObject
{
    public ObservableCollection<HttpRequestModel> SavedRequests { get; } = new();

    [ObservableProperty]
    private HttpRequestModel? _selectedRequest;

    public event EventHandler<HttpRequestModel>? RequestSelected;

    public void LoadSavedRequests(List<HttpRequestModel> requests)
    {
        SavedRequests.Clear();
        foreach (var r in requests)
            SavedRequests.Add(r);
    }

    public List<HttpRequestModel> GetSavedRequests() => SavedRequests.ToList();

    public void SelectAndLoad(HttpRequestModel request)
    {
        SelectedRequest = request;
        RequestSelected?.Invoke(this, request);
    }

    [RelayCommand]
    private void LoadRequest(object? param)
    {
        if (param is HttpRequestModel req)
            SelectAndLoad(req);
    }

    [RelayCommand]
    private void CopyRequest(object? param)
    {
        if (param is HttpRequestModel req)
        {
            var copy = new HttpRequestModel
            {
                Name = req.Name + " (copy)",
                Method = req.Method,
                Url = req.Url,
                Headers = req.Headers.Select(h => new RequestHeader { Key = h.Key, Value = h.Value, IsEnabled = h.IsEnabled }).ToList(),
                Parameters = req.Parameters.Select(p => new RequestParameter { Key = p.Key, Value = p.Value, IsEnabled = p.IsEnabled }).ToList(),
                BodyType = req.BodyType,
                BodyContent = req.BodyContent,
                AuthType = req.AuthType,
                AuthUsername = req.AuthUsername,
                AuthPassword = req.AuthPassword,
                AuthToken = req.AuthToken
            };
            SavedRequests.Add(copy);
        }
    }

    [RelayCommand]
    private void RemoveRequest(object? param)
    {
        if (param is HttpRequestModel req)
            SavedRequests.Remove(req);
    }
}
