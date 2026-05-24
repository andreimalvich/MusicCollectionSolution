using CommunityToolkit.Mvvm.ComponentModel;

namespace MusicCollection.AvaloniaUI.ViewModels;

// Вспомогательные легковесные UI-модели для динамических списков формы ввода
public partial class UiTrackModel : ObservableObject
{
    [ObservableProperty]
    public partial int Number { get; set; }

    [ObservableProperty]
    public partial string Title { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string DurationStr { get; set; } = "03:30"; // Дефолт маска ввода
}
