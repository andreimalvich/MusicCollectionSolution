// MusicCollection.AvaloniaUI/ViewModels/BatchLoadTracksWindowViewModel.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicCollection.Application.Albums.Queries.GetAlbumDetails;
using MusicCollection.Application.Tracks.Commands.AddTracksBatch;

namespace MusicCollection.AvaloniaUI.ViewModels;

public partial class BatchLoadTracksWindowViewModel : ViewModelBase
{
    private readonly IAddTracksBatchCommandService _addTracksBatchService;
    private readonly AlbumDetailsDto _albumDetails;

    // Список дисков альбома для выпадающего списка (ComboBox)
    public ObservableCollection<DiscDetailsDto> Discs { get; } = [];

    [ObservableProperty] public partial DiscDetailsDto? SelectedDisc { get; set; }
    [ObservableProperty] public partial string RawTrackListText { get; set; } = string.Empty;

    public string AlbumTitle => _albumDetails.Title;

    public event Action? RequestClose;

    public BatchLoadTracksWindowViewModel(
        IAddTracksBatchCommandService addTracksBatchService,
        AlbumDetailsDto albumDetails)
    {
        _addTracksBatchService = addTracksBatchService;
        _albumDetails = albumDetails;

        // Наполняем ComboBox дисками этого альбома
        foreach (var disc in _albumDetails.Discs.OrderBy(d => d.DiscNumber))
        {
            Discs.Add(disc);
        }
        SelectedDisc = Discs.FirstOrDefault();
    }

    [RelayCommand]
    private async Task ImportAsync()
    {
        if (SelectedDisc == null || string.IsNullOrWhiteSpace(RawTrackListText)) return;

        // Разрезаем большой текст из TextBox на отдельные строки
        var lines = RawTrackListText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        var importItems = new List<TrackImportItemDto>();
        int trackCounter = 1;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            try
            {
                // Ожидаем формат строки: "Название песни - 04:52" или "01. Название - 04:52"
                var parts = line.Split('-');
                if (parts.Length != 2) continue;

                var leftPart = parts[0].Trim();
                var rightPart = parts[1].Trim(); // Время, например "04:52"

                // Отрезаем номер трека в начале (например, "01. Thunderstruck" -> "Thunderstruck")
                string title = leftPart;
                if (leftPart.Contains('.'))
                {
                    var dotIndex = leftPart.IndexOf('.');
                    title = leftPart[(dotIndex + 1)..].Trim();
                }

                // Превращаем строку "ММ:СС" в системный TimeSpan
                var duration = TimeSpan.Parse($"00:{rightPart}");

                importItems.Add(new TrackImportItemDto(trackCounter++, title, duration));
            }
            catch
            {
                // Если строка не распарсилась — добавляем как заглушку, чтобы импорт не падал
                importItems.Add(new TrackImportItemDto(trackCounter++, $"[Ошибка парсинга] {line}", TimeSpan.Zero));
            }
        }

        if (importItems.Count > 0)
        {
            // Отправляем всю пачку треков в наш готовый сервис на слое Application
            var command = new AddTracksBatchCommand(SelectedDisc.Id, importItems);
            await _addTracksBatchService.ExecuteAsync(command);

            RequestClose?.Invoke();
        }
    }
}
