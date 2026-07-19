using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kalevera.Views;

public partial class SettingsDialog : UserControl
{
    public SettingsDialog()
    {
        InitializeComponent();
    }

    private void GitHubLink_Click(object sender, MouseButtonEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://github.com/exsarorrayzer",
            UseShellExecute = true
        });
    }
}
