using Microsoft.EntityFrameworkCore;
using MusicCollection.Application.Common.Interfaces;

namespace MusicCollection.Application.Albums.Queries.GetAlbumsCarousel;

public class GetAlbumsCarouselQuery(IApplicationDbContext context) : IGetAlbumsCarouselQuery
{
    public async Task<List<AlbumCarouselItemDto>> ExecuteAsync(
        string? searchFilter = null,
        int? artistId = null,
        CancellationToken ct = default)
    {
        var query = context.Albums.AsNoTracking();

        if (artistId.HasValue)
        {
            query = query.Where(a => a.ArtistId == artistId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchFilter))
        {
            searchFilter = searchFilter.Trim();
            query = query.Where(a => a.Title.Contains(searchFilter) || a.Artist.Name.Contains(searchFilter));
        }

        return await query
            .OrderBy(a => a.Title)
            .Select(a => new AlbumCarouselItemDto(
                a.Id,
                a.Title,
                a.Artist.Name,
                a.ReleaseYear, // Передаем год выпуска из БД на лету
                a.Image != null ? a.Image.Data : null
            ))
            .ToListAsync(ct);
    }
}
