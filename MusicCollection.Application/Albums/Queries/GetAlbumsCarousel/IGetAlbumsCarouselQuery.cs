namespace MusicCollection.Application.Albums.Queries.GetAlbumsCarousel;

public interface IGetAlbumsCarouselQuery
{
    Task<List<AlbumCarouselItemDto>> ExecuteAsync(
        string? searchFilter = null,
        int? artistId = null,
        CancellationToken ct = default);
}
