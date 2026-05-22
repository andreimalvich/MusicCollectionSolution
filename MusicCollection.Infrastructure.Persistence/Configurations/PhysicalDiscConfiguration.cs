using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicCollection.Domain.Entities;

namespace MusicCollection.Infrastructure.Persistence.Configurations;

public class PhysicalDiscConfiguration : IEntityTypeConfiguration<PhysicalDisc>
{
    public void Configure(EntityTypeBuilder<PhysicalDisc> builder)
    {
        builder.ToTable("Discs");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.DiscNumber)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(d => d.DiscName)
            .HasMaxLength(200);

        // 4. Настройка отношений
        // Многие-к-одному: диск принадлежит конкретному альбому
        builder.HasOne(d => d.Album)
            .WithMany(a => a.PhysicalDiscs)
            .HasForeignKey(d => d.AlbumId)
            .OnDelete(DeleteBehavior.Cascade);

        // Один-ко-многим: на диске содержится коллекция треков
        builder.HasMany(d => d.Tracks)
            .WithOne(t => t.PhysicalDisc)
            .HasForeignKey(t => t.PhysicalDiscId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
