using Avalonia.Controls;
using Avalonia.Interactivity;
using MusicCollection.AvaloniaUI.ViewModels;

namespace MusicCollection.AvaloniaUI.Views;

public partial class BatchLoadTracksWindow : Window
{
    public BatchLoadTracksWindow()
    {
        InitializeComponent();
    }

    public BatchLoadTracksWindow(BatchLoadTracksWindowViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
        viewModel.RequestClose += () => Close(true);
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}
