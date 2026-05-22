namespace MusicCollection.Application.Artists.Queries.GetArtistsList;

public interface IGetArtistsListQuery
{
    Task<List<ArtistLookupDto>> ExecuteAsync(CancellationToken ct = default);
}
