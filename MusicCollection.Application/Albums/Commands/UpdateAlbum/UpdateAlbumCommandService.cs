using MusicCollection.Application.Common.Interfaces;

namespace MusicCollection.Application.Albums.Commands.UpdateAlbum;

public class UpdateAlbumCommandService(IApplicationDbContext context) : IUpdateAlbumCommandService
{
    public async Task ExecuteAsync(UpdateAlbumCommand command, CancellationToken ct = default)
    {
        // Извлекаем существующий альбом из MS SQL Server
        var album = await context.Albums.FindAsync([command.Id], ct)
            ?? throw new KeyNotFoundException($"Альбом с идентификатором {command.Id} не найден в коллекции.");

        // Обновляем свойства доменной сущности чистыми данными
        album.Title = command.Title.Trim();
        album.ReleaseYear = command.ReleaseYear;
        album.CatalogNumber = command.CatalogNumber?.Trim();
        album.Label = command.Label?.Trim();
        album.Packaging = command.Packaging;

        // Сохраняем изменения
        await context.SaveChangesAsync(ct);
    }
}
