namespace MusicCollection.Application.Albums.Commands.CreateAlbum;

public interface ICreateAlbumCommandService
{
    Task<int> ExecuteAsync(CreateAlbumCommand command, CancellationToken ct = default);
}
