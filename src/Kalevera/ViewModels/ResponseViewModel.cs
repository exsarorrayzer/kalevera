using CommunityToolkit.Mvvm.ComponentModel;
using Kalevera.Models;

namespace Kalevera.ViewModels;

public partial class ResponseViewModel : ObservableObject
{
    [ObservableProperty]
    private HttpResponseModel? _response;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasResponse;

    public void SetResponse(HttpResponseModel response)
    {
        Response = response;
        HasResponse = true;
        IsLoading = false;
    }

    public void Clear()
    {
        Response = null;
        HasResponse = false;
    }
}
