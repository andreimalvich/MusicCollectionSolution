using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MusicCollection.Application.Albums.Commands.CreateAlbum;
using MusicCollection.Application.Albums.Commands.UpdateAlbum;
using MusicCollection.Application.Albums.Queries.GetAlbumDetails;
using MusicCollection.Application.Artists.Queries.GetArtistsList;
using MusicCollection.Application.Common.Interfaces;
using MusicCollection.Domain.Entities;

namespace MusicCollection.AvaloniaUI.ViewModels;

public partial class AddAlbumWindowViewModel : ViewModelBase
{
    private readonly IGetArtistsListQuery _artistsQuery;
    private readonly ICreateAlbumCommandService _createAlbumService;
    private readonly IUpdateAlbumCommandService _updateService;
    private readonly IGetAlbumDetailsQuery _detailsQuery;

    // Храним ID редактируемого альбома (0, если это создание)
    private readonly int _editingAlbumId;

    public AddAlbumWindowViewModel(
        IGetArtistsListQuery artistsQuery,
        ICreateAlbumCommandService createAlbumService,
        IUpdateAlbumCommandService updateService,
        IGetAlbumDetailsQuery detailsQuery,
        int albumId = 0)
    {
        // Привязываем переданные из DI сервисы к полям нашего класса
        _artistsQuery = artistsQuery;
        _createAlbumService = createAlbumService;
        _updateService = updateService;
        _detailsQuery = detailsQuery;
        _editingAlbumId = albumId;

        _ = InitializeAsync();

        if (_editingAlbumId == 0)
        {
            AddDisc();
        }
    }

    // Событие для закрытия окна из кода после успешного сохранения
    public event Action? RequestClose;

    // Списки для выпадающих меню (Исполнители и Форматы упаковки)
    public ObservableCollection<ArtistLookupDto> Artists { get; } = [];

    public Format[] Formats { get; } = Enum.GetValues<Format>();

    // Реактивные поля формы (Partial Properties C# 12)
    [ObservableProperty]
    public partial string Title { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int ReleaseYear { get; set; } = DateTime.Today.Year;

    [ObservableProperty]
    public partial string? CatalogNumber { get; set; }

    [ObservableProperty]
    public partial string? Label { get; set; }

    [ObservableProperty]
    public partial Format SelectedFormat { get; set; } = Format.JewelCase;

    [ObservableProperty]
    public partial ArtistLookupDto? SelectedArtist { get; set; }

    [ObservableProperty]
    public partial byte[]? CoverImageBytes { get; set; }

    // Динамическая коллекция дисков для формы
    public ObservableCollection<UiDiscModel> Discs { get; } = [];

    // Динамический заголовок окна в зависимости от режима
    public string WindowTitle => _editingAlbumId > 0 ? "Редактирование альбома" : "Добавление нового альбома в коллекцию";

    // Свойство для сокрытия секции треков при обычном редактировании метаданных
    public bool IsTrackEditingVisible => _editingAlbumId == 0;

    [ObservableProperty]
    public partial string? ArtistSearchText { get; set; }

    private async Task InitializeAsync()
    {
        // Загружаем список артистов для ComboBox
        var list = await _artistsQuery.ExecuteAsync();
        foreach (var artist in list)
        {
            Artists.Add(artist);
        }

        // Если это РЕДАКТИРОВАНИЕ — подгружаем данные альбома из БД
        if (_editingAlbumId > 0)
        {
            var albumDetails = await _detailsQuery.ExecuteAsync(_editingAlbumId);
            if (albumDetails != null)
            {
                Title = albumDetails.Title;
                ReleaseYear = albumDetails.ReleaseYear;
                CatalogNumber = albumDetails.CatalogNumber;
                Label = albumDetails.Label;
                CoverImageBytes = albumDetails.CoverImageData;

                // Парсим формат упаковки из строки обратно в Enum
                if (Enum.TryParse<Format>(albumDetails.Packaging, out var format))
                {
                    SelectedFormat = format;
                }

                // Находим текущего артиста в списке по имени
                SelectedArtist = Artists.FirstOrDefault(a => a.Name == albumDetails.ArtistName);
            }
        }
        else
        {
            // Если это СОЗДАНИЕ — подставляем дефолты
            SelectedArtist = Artists.FirstOrDefault();
            AddDisc();
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        // Проверяем: либо выбран артист из списка, либо вбит текст руками
        string? artistName = SelectedArtist?.Name ?? ArtistSearchText;

        if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(artistName))
        {
            return;
        }

        try
        {
            int finalArtistId = SelectedArtist?.Id ?? 0;

            // Если финальный ID равен 0, значит это новый артист!
            // Нам нужно сначала создать его в базе данных, чтобы получить рабочий ArtistId
            if (finalArtistId == 0)
            {
                // Для этого временно запрашиваем контекст из DI
                using var scope = App.ServiceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

                var newArtist = new MusicCollection.Domain.Entities.Artist { Name = artistName.Trim() };
                context.Artists.Add(newArtist);
                await context.SaveChangesAsync();

                finalArtistId = newArtist.Id; // Получаем сгенерированный базой ID
            }

            if (_editingAlbumId > 0)
            {
                // Режим редактирования
                var updateCommand = new UpdateAlbumCommand(
                    Id: _editingAlbumId,
                    Title: Title,
                    ReleaseYear: ReleaseYear,
                    CatalogNumber: CatalogNumber,
                    Label: Label,
                    Packaging: SelectedFormat
                );
                await _updateService.ExecuteAsync(updateCommand);
            }
            else
            {
                // Режим создания (передаем finalArtistId)
                var createCommand = new CreateAlbumCommand(
                    Title: Title,
                    ReleaseYear: ReleaseYear,
                    CatalogNumber: CatalogNumber,
                    Label: Label,
                    Packaging: SelectedFormat,
                    ArtistId: finalArtistId, // Используем проверенный ID
                    CoverImage: CoverImageBytes,
                    Discs: Discs.Select(d => new CreateDiscDto(
                        d.DiscNumber,
                        d.DiscName,
                        d.Tracks.Select(t => new CreateTrackDto(t.Number, t.Title, TimeSpan.TryParse($"00:{t.DurationStr}", out var ts) ? ts : TimeSpan.Zero)).ToList()
                    )).ToList()
                );
                await _createAlbumService.ExecuteAsync(createCommand);
            }

            RequestClose?.Invoke();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка сохранения: {ex.Message}");
        }
    }

    private async Task LoadArtistsAsync()
    {
        var list = await _artistsQuery.ExecuteAsync();
        foreach (var artist in list)
        {
            Artists.Add(artist);
        }

        SelectedArtist = Artists.FirstOrDefault();
    }

    [RelayCommand]
    private async Task SelectCoverImageAsync(TopLevel? topLevel)
    {
        if (topLevel == null)
        {
            return;
        }

        // Открываем диалоговое окно выбора файла по стандартам Avalonia
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Выберите обложку альбома",
            AllowMultiple = false, // Только один файл
            FileTypeFilter = [FilePickerFileTypes.ImageAll], // Фильтр: только картинки (jpeg, png, bmp...)
        });

        // Если пользователь выбрал файл
        if (files.Count > 0)
        {
            var file = files[0];

            // Читаем файл в поток и превращаем в массив байт
            using var stream = await file.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);

            // Записываем результат в наше свойство CoverImageBytes, которое уйдет в базу данных
            CoverImageBytes = memoryStream.ToArray();
        }
    }

    [RelayCommand]
    private void AddDisc()
    {
        int nextNumber = Discs.Count + 1;
        var newDisc = new UiDiscModel { DiscNumber = nextNumber };
        newDisc.AddTrack(); // Сразу добавляем один пустой трек в новый диск
        Discs.Add(newDisc);
    }

    [RelayCommand]
    private void RemoveDisc(UiDiscModel disc)
    {
        if (Discs.Count > 1)
        {
            Discs.Remove(disc);
        }
    }
}
