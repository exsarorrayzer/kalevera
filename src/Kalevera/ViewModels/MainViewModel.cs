using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kalevera.Models;
using Kalevera.Services;

namespace Kalevera.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly HttpClientService _httpService = new();
    private readonly StorageService _storage = new();

    public RequestViewModel RequestVM { get; } = new();
    public ResponseViewModel ResponseVM { get; } = new();
    public SavedRequestsViewModel SavedRequestsVM { get; } = new();
    public EnvironmentsViewModel EnvironmentsVM { get; } = new();
    public FetchConverterViewModel FetchConverterVM { get; } = new();
    public SettingsViewModel SettingsVM { get; } = new();

    public ObservableCollection<RequestHistoryEntry> History { get; } = new();

    [ObservableProperty]
    private string _windowTitle = "Kalevera";

    [ObservableProperty]
    private bool _isHistoryPanelOpen;

    [ObservableProperty]
    private RequestHistoryEntry? _selectedHistoryEntry;

    public MainViewModel()
    {
        SavedRequestsVM.RequestSelected += (_, req) => RequestVM.LoadRequest(req);
        FetchConverterVM.RequestParsed += (_, req) =>
        {
            RequestVM.LoadRequest(req);
        };
    }

    public void LoadData()
    {
        var savedRequests = _storage.LoadSavedRequests();
        SavedRequestsVM.LoadSavedRequests(savedRequests);

        var environments = _storage.LoadEnvironments();
        EnvironmentsVM.LoadEnvironments(environments);

        var history = _storage.LoadHistory();
        History.Clear();
        foreach (var h in history)
            History.Add(h);
    }

    public void SaveData()
    {
        _storage.SaveSavedRequests(SavedRequestsVM.GetSavedRequests());
        _storage.SaveEnvironments(EnvironmentsVM.GetEnvironments());
        _storage.SaveHistory(History.ToList());
    }

    [RelayCommand]
    private async Task SendRequestAsync()
    {
        var request = RequestVM.Request;
        if (string.IsNullOrWhiteSpace(request.Url)) return;

        ResponseVM.IsLoading = true;
        ResponseVM.Clear();

        var response = await _httpService.SendRequestAsync(request, EnvironmentsVM.ActiveVariables);
        ResponseVM.SetResponse(response);

        var historyEntry = new RequestHistoryEntry
        {
            Request = new HttpRequestModel
            {
                Name = request.Name,
                Method = request.Method,
                Url = request.Url,
                Headers = new List<RequestHeader>(request.Headers),
                Parameters = new List<RequestParameter>(request.Parameters),
                BodyType = request.BodyType,
                BodyContent = request.BodyContent,
                AuthType = request.AuthType,
                AuthUsername = request.AuthUsername,
                AuthPassword = request.AuthPassword,
                AuthToken = request.AuthToken
            },
            Response = response
        };

        History.Insert(0, historyEntry);
        if (History.Count > 100)
            History.RemoveAt(History.Count - 1);

        SaveData();
    }

    [RelayCommand]
    private void NewRequest()
    {
        RequestVM.LoadRequest(new HttpRequestModel());
        ResponseVM.Clear();
    }

    [RelayCommand]
    private void ToggleHistory()
    {
        IsHistoryPanelOpen = !IsHistoryPanelOpen;
    }

    [RelayCommand]
    private void LoadFromHistory(object? param)
    {
        if (param is RequestHistoryEntry entry)
        {
            RequestVM.LoadRequest(entry.Request);
            if (entry.Response != null)
                ResponseVM.SetResponse(entry.Response);
        }
    }

    [RelayCommand]
    private void OpenFetchConverter()
    {
        FetchConverterVM.IsDialogOpen = true;
    }

    [RelayCommand]
    private void SaveCurrentRequest()
    {
        var request = RequestVM.Request;
        if (string.IsNullOrWhiteSpace(request.Url)) return;

        SavedRequestsVM.SavedRequests.Add(new HttpRequestModel
        {
            Name = request.Name,
            Method = request.Method,
            Url = request.Url,
            Headers = request.Headers.Select(h => new RequestHeader { Key = h.Key, Value = h.Value, IsEnabled = h.IsEnabled }).ToList(),
            Parameters = request.Parameters.Select(p => new RequestParameter { Key = p.Key, Value = p.Value, IsEnabled = p.IsEnabled }).ToList(),
            BodyType = request.BodyType,
            BodyContent = request.BodyContent,
            AuthType = request.AuthType,
            AuthUsername = request.AuthUsername,
            AuthPassword = request.AuthPassword,
            AuthToken = request.AuthToken
        });

        SaveData();
    }
}
