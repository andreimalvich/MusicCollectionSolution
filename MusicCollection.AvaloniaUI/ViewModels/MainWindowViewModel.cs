using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MusicCollection.Application.Albums.Commands.CreateAlbum;
using MusicCollection.Application.Albums.Commands.DeleteAlbum;
using MusicCollection.Application.Albums.Commands.UpdateAlbum;
using MusicCollection.Application.Albums.Queries.GetAlbumDetails;
using MusicCollection.Application.Albums.Queries.GetAlbumsCarousel;
using MusicCollection.Application.Artists.Commands.DeleteArtist;
using MusicCollection.Application.Artists.Queries.GetArtistsList;
using MusicCollection.Application.Tracks.Commands.AddTracksBatch;
using MusicCollection.AvaloniaUI.Views;

namespace MusicCollection.AvaloniaUI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IGetArtistsListQuery _artistsQuery;
    private readonly IGetAlbumsCarouselQuery _carouselQuery;
    private readonly IGetAlbumDetailsQuery _detailsQuery;
    private readonly IDeleteAlbumCommandService _deleteAlbumService;
    private readonly IDeleteArtistCommandService _deleteArtistService;

    public MainWindowViewModel(
        IGetArtistsListQuery artistsQuery,
        IGetAlbumsCarouselQuery carouselQuery,
        IGetAlbumDetailsQuery detailsQuery,
        IDeleteAlbumCommandService deleteAlbumService,
        IDeleteArtistCommandService deleteArtistService)
    {
        _artistsQuery = artistsQuery;
        _carouselQuery = carouselQuery;
        _detailsQuery = detailsQuery;
        _deleteAlbumService = deleteAlbumService;
        _deleteArtistService = deleteArtistService;

        _ = InitializeAsync();
    }

    public ObservableCollection<ArtistLookupDto> Artists { get; } = [];

    public ObservableCollection<AlbumCarouselItemDto> CarouselAlbums { get; } = [];

    [ObservableProperty]
    public partial ArtistLookupDto? SelectedArtist { get; set; }

    [ObservableProperty]
    public partial AlbumCarouselItemDto? SelectedAlbum { get; set; }

    [ObservableProperty]
    public partial AlbumDetailsDto? CurrentAlbumDetails { get; set; }

    [RelayCommand(CanExecute = nameof(CanBatchLoadTracks))]
    private async Task OpenBatchLoadTracksWindowAsync()
    {
        if (CurrentAlbumDetails == null)
        {
            return;
        }

        // Вручную достаем сервис импорта из общего DI-контейнера App
        var batchService = App.ServiceProvider.GetRequiredService<IAddTracksBatchCommandService>();

        // Передаем в конструктор сервис и DTO текущего открытого альбома
        var batchViewModel = new BatchLoadTracksWindowViewModel(batchService, CurrentAlbumDetails);
        var window = new BatchLoadTracksWindow(batchViewModel);

        if (App.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            var result = await window.ShowDialog<bool>(desktop.MainWindow);

            // Если импорт завершился успехом — принудительно перечитываем треки для дерева деталей экрана!
            if (result && SelectedAlbum != null)
            {
                CurrentAlbumDetails = await _detailsQuery.ExecuteAsync(SelectedAlbum.Id);
            }
        }
    }

    private bool CanBatchLoadTracks() => CurrentAlbumDetails != null;

    // Команда удаления альбома
    // Атрибут CanExecute указывает, что кнопка активна ТОЛЬКО когда SelectedAlbum не равен null
    [RelayCommand(CanExecute = nameof(CanDeleteAlbum))]
    private async Task DeleteAlbumAsync()
    {
        if (SelectedAlbum == null)
        {
            return;
        }

        // Вызываем наш готовый каскадный сервис удаления из слоя Application
        var command = new DeleteAlbumCommand(SelectedAlbum.Id);
        await _deleteAlbumService.ExecuteAsync(command);

        // Плавно обновляем интерфейс: перечитываем карусель из MS SQL Server
        await LoadCarouselAsync();
    }

    // Вспомогательный метод-условие для доступности кнопки удаления
    private bool CanDeleteAlbum() => SelectedAlbum != null;

    /// <summary>
    /// Срабатывает автоматически, когда в карусели кликают на другой альбом,
    /// чтобы кнопка "Удалить" активировалась/деактивировалась налету.
    /// </summary>
    /// <param name="value">Новое выбранное значение альбома.</param>
    partial void OnSelectedAlbumChanged(AlbumCarouselItemDto? value)
    {
        // Подгружаем диски и треки для дерева деталей
        _ = LoadAlbumDetailsAsync();

        // Принудительно заставляем кнопку "Удалить" перепроверить свое состояние доступности
        DeleteAlbumCommand.NotifyCanExecuteChanged();

        // 3. ДОБАВЛЕНИЕ: Активируем/деактивируем кнопку "Редактировать метаданные"
        OpenEditAlbumWindowCommand.NotifyCanExecuteChanged();
    }

    // 3. ДОБАВЛЯЕМ КОМАНДУ УДАЛЕНИЯ АРТИСТА
    [RelayCommand(CanExecute = nameof(CanDeleteArtist))]
    private async Task DeleteArtistAsync()
    {
        if (SelectedArtist == null)
        {
            return;
        }

        // Вызываем сервис удаления из слоя Application
        var command = new DeleteArtistCommand(SelectedArtist.Id);
        await _deleteArtistService.ExecuteAsync(command);

        // Полностью обновляем левую панель и сбрасываем выбор
        Artists.Clear();
        var refreshedArtists = await _artistsQuery.ExecuteAsync();
        foreach (var artist in refreshedArtists)
        {
            Artists.Add(artist);
        }

        SelectedArtist = null; // Это автоматически очистит карусель и детали треков!
    }

    private bool CanDeleteArtist() => SelectedArtist != null;

    // ОБНОВЛЯЕМ ТРИГГЕР ИЗМЕНЕНИЯ АРТИСТА
    // Нам нужно сказать кнопке перепроверить CanExecute, когда кликают по списку
    partial void OnSelectedArtistChanged(ArtistLookupDto? value)
    {
        _ = LoadCarouselAsync();

        // Оповещаем команду удаления артиста об изменениях
        DeleteArtistCommand.NotifyCanExecuteChanged();
    }

    private async Task InitializeAsync()
    {
        var artistsList = await _artistsQuery.ExecuteAsync();
        foreach (var artist in artistsList)
        {
            Artists.Add(artist);
        }

        await LoadCarouselAsync();
    }

    // Вызов окна добавления нового альбома
    [RelayCommand]
    private async Task OpenAddAlbumWindowAsync()
    {
        var window = App.ServiceProvider.GetRequiredService<AddAlbumWindow>();

        if (App.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            var result = await window.ShowDialog<bool>(desktop.MainWindow);

            // Если окно вернуло true (альбом и новый артист успешно сохранены)
            if (result)
            {
                // ПОЧИНКА: Полностью перечитываем список артистов из MS SQL Server
                Artists.Clear();
                var refreshedArtists = await _artistsQuery.ExecuteAsync();
                foreach (var artist in refreshedArtists)
                {
                    Artists.Add(artist);
                }

                // Обновляем карусель альбомов, как и раньше
                await LoadCarouselAsync();
            }
        }
    }

    [RelayCommand(CanExecute = nameof(CanEditAlbum))]
    private async Task OpenEditAlbumWindowAsync()
    {
        if (SelectedAlbum == null)
        {
            return;
        }

        // Вручную собираем ViewModel с передачей ID выбранного альбома
        using var scope = App.ServiceProvider.CreateScope();

        // Запрашиваем зависимости из DI вручную для сборки кастомного конструктора
        var artistsQuery = App.ServiceProvider.GetRequiredService<MusicCollection.Application.Artists.Queries.GetArtistsList.IGetArtistsListQuery>();
        var createService = App.ServiceProvider.GetRequiredService<ICreateAlbumCommandService>();
        var updateService = App.ServiceProvider.GetRequiredService<IUpdateAlbumCommandService>();
        var detailsQuery = App.ServiceProvider.GetRequiredService<IGetAlbumDetailsQuery>();

        var editViewModel = new AddAlbumWindowViewModel(artistsQuery, createService, updateService, detailsQuery, SelectedAlbum.Id);
        var window = new AddAlbumWindow(editViewModel);

        if (App.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            var result = await window.ShowDialog<bool>(desktop.MainWindow);
            if (result)
            {
                // Обновляем витрину и блок деталей
                await LoadCarouselAsync();
                if (SelectedAlbum != null)
                {
                    CurrentAlbumDetails = await _detailsQuery.ExecuteAsync(SelectedAlbum.Id);
                }
            }
        }
    }

    private bool CanEditAlbum() => SelectedAlbum != null;

    private async Task LoadCarouselAsync()
    {
        CarouselAlbums.Clear();
        CurrentAlbumDetails = null;

        var albums = await _carouselQuery.ExecuteAsync(artistId: SelectedArtist?.Id);
        foreach (var album in albums)
        {
            CarouselAlbums.Add(album);
        }
    }

    private async Task LoadAlbumDetailsAsync()
    {
        if (SelectedAlbum == null)
        {
            CurrentAlbumDetails = null;
            return;
        }

        CurrentAlbumDetails = await _detailsQuery.ExecuteAsync(SelectedAlbum.Id);

        OpenBatchLoadTracksWindowCommand.NotifyCanExecuteChanged();
    }
}
