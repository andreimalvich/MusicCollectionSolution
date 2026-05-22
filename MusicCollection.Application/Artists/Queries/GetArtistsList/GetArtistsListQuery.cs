using Microsoft.EntityFrameworkCore;
using MusicCollection.Application.Common.Interfaces;

namespace MusicCollection.Application.Artists.Queries.GetArtistsList;

public class GetArtistsListQuery(IApplicationDbContext context) : IGetArtistsListQuery
{
    public async Task<List<ArtistLookupDto>> ExecuteAsync(CancellationToken ct = default)
    {
        return await context.Artists
            .AsNoTracking() // Отключаем Change Tracking для высокой скорости
            .OrderBy(a => a.Name) // Сортировка по алфавиту на стороне MS SQL Server
            .Select(a => new ArtistLookupDto(a.Id, a.Name)) // Проекция только нужных полей
            .ToListAsync(ct);
    }
}
