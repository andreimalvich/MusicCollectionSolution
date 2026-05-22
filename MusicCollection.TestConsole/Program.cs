#pragma warning disable SA1118

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MusicCollection.Application;
using MusicCollection.Application.Albums.Commands.CreateAlbum;
using MusicCollection.Application.Albums.Queries.GetAlbumDetails;
using MusicCollection.Application.Albums.Queries.GetAlbumsCarousel;
using MusicCollection.Application.Common.Interfaces;
using MusicCollection.Domain.Entities;
using MusicCollection.Infrastructure.Persistence;
using static System.Console;

WriteLine("=== Старт интеграционного теста MusicCollection (.NET 10) ===");

// 1. Настройка строки подключения к вашему MS SQL Server
const string connectionString = @"Server=(localdb)\mssqllocaldb;Database=MusicCDDB;Trusted_Connection=True;TrustServerCertificate=True";

// 2. Инициализация DI-контейнера (точно так же, как будет в Avalonia)
var services = new ServiceCollection();
services.AddPersistenceServices(connectionString); // Подключаем DAL
services.AddApplicationServices();                 // Подключаем бизнес-логику

var serviceProvider = services.BuildServiceProvider();

try
{
    // Используем Scope для изоляции транзакции, как в реальном приложении
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

    // ==========================================
    // ШАГ 1: Подготовка данных (Создаем Артиста)
    // ==========================================
    WriteLine("\n[1/4] Создание артиста...");
    var artist = new Artist { Name = "Pink Floyd" };
    context.Artists.Add(artist);
    await context.SaveChangesAsync();
    WriteLine($"Артист {artist.Name} успешно создан с ID: {artist.Id}");

    // ==========================================
    // ШАГ 2: Тест валидации и массовой записи (Command)
    // ==========================================
    WriteLine("\n[2/4] Тестирование создания многодискового альбома (Box Set)...");
    var createAlbumCommandService = scope.ServiceProvider.GetRequiredService<ICreateAlbumCommandService>();

    // Формируем команду (имитируем заполнение формы в GUI)
    var command = new CreateAlbumCommand(
        Title: "The Wall (Deluxe Edition)",
        ReleaseYear: 1979,
        CatalogNumber: "7243-8-31243-2-9",
        Label: "EMI",
        Packaging: Format.BoxSet, // Подарочный бокс
        ArtistId: artist.Id,
        CoverImage: [1, 2, 3, 4, 5], // Имитируем байты картинки
        Discs: [
            new CreateDiscDto(1, "In The Flesh?", [
                new CreateTrackDto(1, "In the Flesh?", TimeSpan.FromMinutes(3).Add(TimeSpan.FromSeconds(16))),
                new CreateTrackDto(2, "The Thin Ice", TimeSpan.FromMinutes(2).Add(TimeSpan.FromSeconds(27)))
            ]),
            new CreateDiscDto(2, "Run Like Hell", [
                new CreateTrackDto(1, "Hey You", TimeSpan.FromMinutes(4).Add(TimeSpan.FromSeconds(40))),
                new CreateTrackDto(2, "Comfortably Numb", TimeSpan.FromMinutes(6).Add(TimeSpan.FromSeconds(22)))
            ])
        ]
    );

    // Выполняем команду записи
    int newAlbumId = await createAlbumCommandService.ExecuteAsync(command);
    WriteLine($"Альбом успешно сохранен в MS SQL Server! Сгенерированный AlbumId: {newAlbumId}");

    // ==========================================
    // ШАГ 3: Тест чтения витрины (Carousel Query)
    // ==========================================
    Console.WriteLine("\n[3/4] Тестирование запроса витрины (Карусели)...");
    var carouselQuery = scope.ServiceProvider.GetRequiredService<IGetAlbumsCarouselQuery>();

    var carouselItems = await carouselQuery.ExecuteAsync();
    WriteLine($"Витрина вернула {carouselItems.Count} альбом(а):");
    foreach (var item in carouselItems)
    {
        WriteLine($" -> [{item.ReleaseYear}] {item.ArtistName} - {item.Title} (Обложка: {item.CoverImageData?.Length} байт)");
    }

    // ==========================================
    // ШАГ 4: Тест детального чтения с группировкой (Details Query)
    // ==========================================
    WriteLine("\n[4/4] Тестирование детального запроса альбома с иерархией треков...");
    var detailsQuery = scope.ServiceProvider.GetRequiredService<IGetAlbumDetailsQuery>();

    var albumDetails = await detailsQuery.ExecuteAsync(newAlbumId);
    if (albumDetails != null)
    {
        WriteLine($"\nАльбом: {albumDetails.Title}");
        WriteLine($"Исполнитель: {albumDetails.ArtistName}");
        WriteLine($"Лейбл: {albumDetails.Label} | Упаковка: {albumDetails.Packaging}");
        WriteLine(new string('-', 40));

        // Выводим диски и треки по порядку, проверяя нашу группировку
        foreach (var disc in albumDetails.Discs)
        {
            WriteLine($"💿 Диск {disc.DiscNumber}: {disc.DiscName}");
            foreach (var track in disc.Tracks)
            {
                Console.WriteLine($"   {track.Number:D2}. {track.Title} [{track.Duration}]");
            }
        }
    }
}
catch (ValidationException valEx)
{
    // Если FluentValidation найдет ошибки, мы поймаем их здесь
    ForegroundColor = ConsoleColor.Red;
    WriteLine("\n❌ Ошибка валидации данных:");
    foreach (var error in valEx.Errors)
    {
        WriteLine($" - Поле '{error.PropertyName}': {error.ErrorMessage}");
    }

    ResetColor();
}
catch (Exception ex)
{
    ForegroundColor = ConsoleColor.Red;
    WriteLine($"\n❌ Непредвиденная ошибка теста: {ex.Message}");
    if (ex.InnerException != null)
    {
        WriteLine($"Внутренняя ошибка: {ex.InnerException.Message}");
    }

    ResetColor();
}

WriteLine("\n=== Тест завершен. Нажмите любую клавишу для выхода ===");
ReadKey();
