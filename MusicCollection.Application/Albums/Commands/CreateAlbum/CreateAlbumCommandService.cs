using MusicCollection.Application.Common.Interfaces;
using MusicCollection.Domain.Entities;

namespace MusicCollection.Application.Albums.Commands.CreateAlbum;

public class CreateAlbumCommandService(IApplicationDbContext context) : ICreateAlbumCommandService
{
    public async Task<int> ExecuteAsync(CreateAlbumCommand command, CancellationToken ct = default)
    {
        // Создаем корневую сущность альбома
        var album = new Album
        {
            Title = command.Title.Trim(),
            ReleaseYear = command.ReleaseYear,
            CatalogNumber = command.CatalogNumber?.Trim(),
            Label = command.Label?.Trim(),
            Packaging = command.Packaging,
            ArtistId = command.ArtistId,
        };

        // Если пользователь прикрепил обложку — создаем сущность изображения
        if (command.CoverImage is { Length: > 0 })
        {
            album.Image = new AlbumImage { Data = command.CoverImage };
        }

        // Наполняем альбом дисками и треками из C# 12 коллекций
        foreach (var discDto in command.Discs)
        {
            var disc = new PhysicalDisc
            {
                DiscNumber = discDto.DiscNumber,
                DiscName = discDto.DiscName?.Trim(),
            };

            foreach (var trackDto in discDto.Tracks)
            {
                disc.Tracks.Add(new Track
                {
                    Number = trackDto.Number,
                    Title = trackDto.Title.Trim(),
                    Duration = trackDto.Duration,
                });
            }

            album.PhysicalDiscs.Add(disc);
        }

        // Отправляем всю иерархию в базу данных
        context.Albums.Add(album);
        await context.SaveChangesAsync(ct); // EF Core сам сохранит все связи и вернет сгенерированный Id

        return album.Id;
    }
}