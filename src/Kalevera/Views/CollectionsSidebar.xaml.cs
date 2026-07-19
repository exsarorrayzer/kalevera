using System.Windows;
using System.Windows.Controls;

namespace Kalevera.Views;

public partial class CollectionsSidebar : UserControl
{
    public CollectionsSidebar()
    {
        InitializeComponent();
    }

    private void HistoryToggle_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.MainViewModel mainVm)
        {
            if (mainVm.ToggleHistoryCommand.CanExecute(null))
                mainVm.ToggleHistoryCommand.Execute(null);
        }
    }
}
