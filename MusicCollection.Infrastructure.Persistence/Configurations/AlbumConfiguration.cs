using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicCollection.Domain.Entities;

namespace MusicCollection.Infrastructure.Persistence.Configurations;

public class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> builder)
    {
        builder.ToTable("Albums");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(a => a.CatalogNumber)
            .HasMaxLength(50);

        builder.Property(a => a.Label)
            .HasMaxLength(150);

        builder.Property(a => a.Packaging)
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(Domain.Entities.Format.JewelCase);

        // Отношения и каскадные удаления
        // Многие-к-одному: у Альбома один Артист
        builder.HasOne(a => a.Artist)
            .WithMany(a => a.Albums)
            .HasForeignKey(a => a.ArtistId)
            .OnDelete(DeleteBehavior.Cascade);

        // Один-ко-многим: у Альбома может быть несколько физических дисков (2CD, Box Set)
        builder.HasMany(a => a.PhysicalDiscs)
            .WithOne(d => d.Album)
            .HasForeignKey(d => d.AlbumId)
            .OnDelete(DeleteBehavior.Cascade); // Удаление альбома чистит и его диски
    }
}
