using Avalonia.Controls;
using Avalonia.Interactivity;
using MusicCollection.AvaloniaUI.ViewModels;

namespace MusicCollection.AvaloniaUI.Views;

public partial class AddAlbumWindow : Window
{
    public AddAlbumWindow()
    {
        InitializeComponent();
    }

    public AddAlbumWindow(AddAlbumWindowViewModel viewModel)
        : this()
    {
        DataContext = viewModel;

        // Подписываемся на событие закрытия из ViewModel
        viewModel.RequestClose += () => Close(true);
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}
