namespace MusicCollection.Application.Albums.Commands.UpdateAlbum;

public interface IUpdateAlbumCommandService
{
    Task ExecuteAsync(UpdateAlbumCommand command, CancellationToken ct = default);
}