using MusicCollection.Application.Common.Interfaces;

namespace MusicCollection.Application.Albums.Commands.DeleteAlbum;

public class DeleteAlbumCommandService(IApplicationDbContext context) : IDeleteAlbumCommandService
{
    public async Task ExecuteAsync(DeleteAlbumCommand command, CancellationToken ct = default)
    {
        var album = await context.Albums.FindAsync([command.Id], ct)
            ?? throw new KeyNotFoundException($"Альбом с ID {command.Id} не найден.");

        // Удаление по цепочке AlbumImages->Discs->Tracks.
        context.Albums.Remove(album);

        await context.SaveChangesAsync(ct);
    }
}
