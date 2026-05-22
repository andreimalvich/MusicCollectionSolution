namespace MusicCollection.Application.Albums.Queries.GetAlbumDetails;

public interface IGetAlbumDetailsQuery
{
    Task<AlbumDetailsDto?> ExecuteAsync(int albumId, CancellationToken ct = default);
}
