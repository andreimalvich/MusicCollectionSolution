using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicCollection.Domain.Entities;

namespace MusicCollection.Infrastructure.Persistence.Configurations;

public class AlbumImageConfiguration : IEntityTypeConfiguration<AlbumImage>
{
    public void Configure(EntityTypeBuilder<AlbumImage> builder)
    {
        builder.ToTable("AlbumImages");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Data)
            .IsRequired()
            .HasColumnType("varbinary(max)");

        builder.HasOne(i => i.Album)
            .WithOne(a => a.Image)
            .HasForeignKey<AlbumImage>(i => i.AlbumId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
