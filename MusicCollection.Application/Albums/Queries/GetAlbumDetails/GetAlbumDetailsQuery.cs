using Microsoft.EntityFrameworkCore;
using MusicCollection.Application.Common.Interfaces;

namespace MusicCollection.Application.Albums.Queries.GetAlbumDetails;

public class GetAlbumDetailsQuery(IApplicationDbContext context) : IGetAlbumDetailsQuery
{
    public async Task<AlbumDetailsDto?> ExecuteAsync(int albumId, CancellationToken ct = default)
    {
        return await context.Albums
            .AsNoTracking() // Отключаем кэш EF для максимальной скорости чтения
            .Where(a => a.Id == albumId)
            .Select(a => new AlbumDetailsDto(
                a.Id,
                a.Title,
                a.Artist.Name,
                a.ReleaseYear,
                a.CatalogNumber,
                a.Label,
                a.Packaging.ToString(),
                a.Image != null ? a.Image.Data : null,

                // Проекция и сортировка физических дисков по порядку
                a.PhysicalDiscs
                    .OrderBy(d => d.DiscNumber)
                    .Select(d => new DiscDetailsDto(
                        d.Id,
                        d.DiscNumber,
                        d.DiscName,

                        // Проекция и сортировка треков внутри конкретного диска
                        d.Tracks
                            .OrderBy(t => t.Number)
                            .Select(t => new TrackDetailsDto(
                                t.Id,
                                t.Number,
                                t.Title,

                                // Форматируем TimeSpan в строку "ММ:СС" на уровне SQL
                                $"{t.Duration.Minutes:D2}:{t.Duration.Seconds:D2}"
                            ))
                            .ToList()
                    ))
                    .ToList()
            ))
            .FirstOrDefaultAsync(ct);
    }
}
