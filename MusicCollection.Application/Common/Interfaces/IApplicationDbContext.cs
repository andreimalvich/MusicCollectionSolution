using Microsoft.EntityFrameworkCore;
using MusicCollection.Domain.Entities;

namespace MusicCollection.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Artist> Artists { get; }

    DbSet<Album> Albums { get; }

    DbSet<AlbumImage> AlbumImages { get; }

    DbSet<PhysicalDisc> Discs { get; }

    DbSet<Track> Tracks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
