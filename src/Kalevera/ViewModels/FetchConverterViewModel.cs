using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kalevera.Models;
using Kalevera.Services;

namespace Kalevera.ViewModels;

public partial class FetchConverterViewModel : ObservableObject
{
    [ObservableProperty]
    private string _fetchInput = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isDialogOpen;

    public HttpRequestModel? Result { get; private set; }

    public event EventHandler<HttpRequestModel>? RequestParsed;

    [RelayCommand]
    private void Parse()
    {
        HasError = false;
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(FetchInput))
        {
            HasError = true;
            ErrorMessage = "Paste a fetch command first.";
            return;
        }

        try
        {
            Result = FetchConverter.Parse(FetchInput);
            RequestParsed?.Invoke(this, Result);
            IsDialogOpen = false;
            FetchInput = string.Empty;
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private void Close()
    {
        IsDialogOpen = false;
        HasError = false;
        ErrorMessage = string.Empty;
    }
}
