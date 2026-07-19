using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kalevera.Models;

namespace Kalevera.ViewModels;

public partial class CollectionsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Collection> _collections = new();

    [ObservableProperty]
    private Collection? _selectedCollection;

    [ObservableProperty]
    private HttpRequestModel? _selectedRequest;

    public event EventHandler<HttpRequestModel>? RequestSelected;

    [RelayCommand]
    private void AddCollection()
    {
        Collections.Add(new Collection { Name = $"Collection {Collections.Count + 1}" });
    }

    [RelayCommand]
    private void RemoveCollection(object? param)
    {
        if (param is Collection c)
            Collections.Remove(c);
    }

    [RelayCommand]
    private void AddRequest()
    {
        if (SelectedCollection == null) return;
        var req = new HttpRequestModel { Name = $"Request {SelectedCollection.Requests.Count + 1}" };
        SelectedCollection.Requests.Add(req);
    }

    [RelayCommand]
    private void RemoveRequest(object? param)
    {
        if (param is HttpRequestModel r && SelectedCollection != null)
            SelectedCollection.Requests.Remove(r);
    }

    [RelayCommand]
    private void SelectRequest(object? param)
    {
        if (param is HttpRequestModel req)
        {
            SelectedRequest = req;
            RequestSelected?.Invoke(this, req);
        }
    }

    public void LoadCollections(List<Collection> collections)
    {
        Collections.Clear();
        foreach (var c in collections)
            Collections.Add(c);
    }

    public List<Collection> GetCollections() => Collections.ToList();
}
