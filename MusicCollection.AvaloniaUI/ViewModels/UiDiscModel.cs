using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MusicCollection.AvaloniaUI.ViewModels;

// Вспомогательные легковесные UI-модели для динамических списков формы ввода
public partial class UiDiscModel : ObservableObject
{
    [ObservableProperty]
    public partial int DiscNumber { get; set; }

    [ObservableProperty]
    public partial string? DiscName { get; set; }

    public ObservableCollection<UiTrackModel> Tracks { get; } = [];

    [RelayCommand]
    public void AddTrack()
    {
        Tracks.Add(new UiTrackModel { Number = Tracks.Count + 1 });
    }

    [RelayCommand]
    public void RemoveTrack(UiTrackModel track)
    {
        if (Tracks.Count > 1)
        {
            Tracks.Remove(track);
        }
    }
}
