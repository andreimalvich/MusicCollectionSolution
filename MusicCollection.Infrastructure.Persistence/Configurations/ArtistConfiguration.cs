using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicCollection.Domain.Entities;

namespace MusicCollection.Infrastructure.Persistence.Configurations;

public class ArtistConfiguration : IEntityTypeConfiguration<Artist>
{
    public void Configure(EntityTypeBuilder<Artist> builder)
    {
        builder.ToTable("Artists");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasMany(a => a.Albums)
            .WithOne(a => a.Artist)
            .HasForeignKey(a => a.ArtistId)
            .OnDelete(DeleteBehavior.Cascade); // При удалении артиста удаляются его альбомы
    }
}
