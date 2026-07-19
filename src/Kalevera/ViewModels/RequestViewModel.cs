using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kalevera.Models;

namespace Kalevera.ViewModels;

public partial class RequestViewModel : ObservableObject
{
    [ObservableProperty]
    private HttpRequestModel _request = new();

    [ObservableProperty]
    private int _selectedTabIndex;

    public ObservableCollection<RequestParameter> Parameters { get; private set; } = new();
    public ObservableCollection<RequestHeader> Headers { get; private set; } = new();

    public HttpMethodType[] Methods { get; } = Enum.GetValues<HttpMethodType>();
    public AuthType[] AuthTypes { get; } = Enum.GetValues<AuthType>();
    public BodyType[] BodyTypes { get; } = Enum.GetValues<BodyType>();

    [RelayCommand]
    private void AddParameter()
    {
        var p = new RequestParameter();
        Parameters.Add(p);
        Request.Parameters.Add(p);
    }

    [RelayCommand]
    private void RemoveParameter(object? param)
    {
        if (param is RequestParameter p)
        {
            Parameters.Remove(p);
            Request.Parameters.Remove(p);
        }
    }

    [RelayCommand]
    private void AddHeader()
    {
        var h = new RequestHeader();
        Headers.Add(h);
        Request.Headers.Add(h);
    }

    [RelayCommand]
    private void RemoveHeader(object? param)
    {
        if (param is RequestHeader h)
        {
            Headers.Remove(h);
            Request.Headers.Remove(h);
        }
    }

    public void LoadRequest(HttpRequestModel request)
    {
        Request = request;
        Parameters = new ObservableCollection<RequestParameter>(request.Parameters);
        Headers = new ObservableCollection<RequestHeader>(request.Headers);
        OnPropertyChanged(nameof(Request));
        OnPropertyChanged(nameof(Parameters));
        OnPropertyChanged(nameof(Headers));
    }
}
