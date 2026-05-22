using Avalonia.Controls;
using MusicCollection.AvaloniaUI.ViewModels;

namespace MusicCollection.AvaloniaUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(MainWindowViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}
