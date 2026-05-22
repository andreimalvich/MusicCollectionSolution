namespace MusicCollection.Application.Artists.Commands.DeleteArtist;

public interface IDeleteArtistCommandService
{
    Task ExecuteAsync(DeleteArtistCommand command, CancellationToken ct = default);
}
