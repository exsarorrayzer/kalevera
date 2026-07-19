using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kalevera.Models;
using Environment = Kalevera.Models.Environment;

namespace Kalevera.ViewModels;

public partial class EnvironmentsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Environment> _environments = new();

    [ObservableProperty]
    private Environment? _activeEnvironment;

    [ObservableProperty]
    private Environment? _selectedEnvironment;

    [ObservableProperty]
    private bool _isDialogOpen;

    public Dictionary<string, string> ActiveVariables =>
        ActiveEnvironment != null ? Services.EnvironmentService.ToDictionary(ActiveEnvironment) : new();

    [RelayCommand]
    private void AddEnvironment()
    {
        var env = new Environment { Name = $"Environment {Environments.Count + 1}" };
        Environments.Add(env);
        ActiveEnvironment ??= env;
    }

    [RelayCommand]
    private void RemoveEnvironment(object? param)
    {
        if (param is Environment e)
        {
            Environments.Remove(e);
            if (ActiveEnvironment == e)
                ActiveEnvironment = Environments.FirstOrDefault();
        }
    }

    [RelayCommand]
    private void OpenDialog(object? param)
    {
        if (param is Environment env)
            SelectedEnvironment = env;
        else
            SelectedEnvironment = null;
        IsDialogOpen = true;
    }

    [RelayCommand]
    private void CloseDialog()
    {
        IsDialogOpen = false;
        SelectedEnvironment = null;
    }

    [RelayCommand]
    private void AddVariable()
    {
        if (SelectedEnvironment != null)
            SelectedEnvironment.Variables.Add(new EnvironmentVariable());
    }

    [RelayCommand]
    private void RemoveVariable(object? param)
    {
        if (param is EnvironmentVariable v && SelectedEnvironment != null)
            SelectedEnvironment.Variables.Remove(v);
    }

    public void LoadEnvironments(List<Environment> environments)
    {
        Environments.Clear();
        foreach (var e in environments)
            Environments.Add(e);
        ActiveEnvironment = Environments.FirstOrDefault();
    }

    public List<Environment> GetEnvironments() => Environments.ToList();
}
