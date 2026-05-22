using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MusicCollection.Application.Common.Interfaces;
using MusicCollection.Domain.Entities;

namespace MusicCollection.Infrastructure.Persistence.Contexts;

public class MusicDbContext(DbContextOptions<MusicDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<Artist> Artists => Set<Artist>();

    public DbSet<Album> Albums => Set<Album>();

    public DbSet<AlbumImage> AlbumImages => Set<AlbumImage>();

    public DbSet<PhysicalDisc> Discs => Set<PhysicalDisc>();

    public DbSet<Track> Tracks => Set<Track>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
