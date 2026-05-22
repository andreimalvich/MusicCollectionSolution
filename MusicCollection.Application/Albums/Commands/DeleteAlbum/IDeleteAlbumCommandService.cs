namespace MusicCollection.Application.Albums.Commands.DeleteAlbum;

public interface IDeleteAlbumCommandService
{
    Task ExecuteAsync(DeleteAlbumCommand command, CancellationToken ct = default);
}