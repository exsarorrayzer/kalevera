using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;

namespace Kalevera.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private static readonly string SettingsPath = System.IO.Path.Combine(
        System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
        "Kalevera", "settings.json");

    [ObservableProperty]
    private string _activeTheme = "Dark Red";

    [ObservableProperty]
    private bool _isDialogOpen;

    public ObservableCollection<ThemeOption> Themes { get; } = new()
    {
        new("Dark Red", "#E63946", "#0D0D0D", "#1A1A1A", "#252525"),
        new("Dark Blue", "#4A90D9", "#0A0E14", "#131820", "#1C2230"),
        new("Dark Purple", "#9B59B6", "#0D0A0F", "#1A1420", "#251E30"),
        new("Dark Green", "#2ECC71", "#0A0F0D", "#141E1A", "#1E2E25"),
        new("OLED Black", "#E63946", "#000000", "#0A0A0A", "#111111"),
    };

    public event EventHandler<string>? ThemeChanged;

    public SettingsViewModel()
    {
        Load();
    }

    [RelayCommand]
    private void OpenSettings()
    {
        IsDialogOpen = true;
    }

    [RelayCommand]
    private void Close()
    {
        IsDialogOpen = false;
    }

    [RelayCommand]
    private void SelectTheme(object? param)
    {
        if (param is ThemeOption theme)
        {
            ActiveTheme = theme.Name;
            Save();
            ThemeChanged?.Invoke(this, theme.Name);
        }
    }

    public void Load()
    {
        try
        {
            if (System.IO.File.Exists(SettingsPath))
            {
                var json = System.IO.File.ReadAllText(SettingsPath);
                var data = JsonConvert.DeserializeObject<SettingsData>(json);
                if (data != null && !string.IsNullOrEmpty(data.ActiveTheme))
                    ActiveTheme = data.ActiveTheme;
            }
        }
        catch { }
    }

    public void Save()
    {
        try
        {
            var dir = System.IO.Path.GetDirectoryName(SettingsPath)!;
            System.IO.Directory.CreateDirectory(dir);
            var json = JsonConvert.SerializeObject(new SettingsData { ActiveTheme = ActiveTheme }, Formatting.Indented);
            System.IO.File.WriteAllText(SettingsPath, json);
        }
        catch { }
    }
}

public class ThemeOption
{
    public string Name { get; }
    public string AccentColor { get; }
    public string PrimaryBg { get; }
    public string SecondaryBg { get; }
    public string TertiaryBg { get; }

    public ThemeOption(string name, string accent, string primary, string secondary, string tertiary)
    {
        Name = name;
        AccentColor = accent;
        PrimaryBg = primary;
        SecondaryBg = secondary;
        TertiaryBg = tertiary;
    }
}

public class SettingsData
{
    public string ActiveTheme { get; set; } = "Dark Red";
}
