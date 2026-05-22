using Microsoft.EntityFrameworkCore;
using MusicCollection.Domain.Entities;

namespace MusicCollection.Infrastructure.Persistence.Configurations;

public class TrackConfiguration : IEntityTypeConfiguration<Track>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Track> builder)
    {
        builder.ToTable("Tracks");

        // 2. Первичный ключ
        builder.HasKey(t => t.Id);

        // 3. Настройка свойств
        builder.Property(t => t.Number)
            .IsRequired(); // Порядковый номер трека на CD (NOT NULL)

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(300); // Ограничение nvarchar(300) для длинных названий песен

        // 4. Оптимизация типа TimeSpan для MS SQL Server
        builder.Property(t => t.Duration)
            .HasColumnType("time(0)") // Сохраняет ЧЧ:ММ:СС без лишних миллисекунд
            .IsRequired();

        // 5. Связь "Многие-к-одному" (настраивается зеркально к PhysicalDisc)
        builder.HasOne(t => t.PhysicalDisc)
            .WithMany(d => d.Tracks)
            .HasForeignKey(t => t.PhysicalDiscId)
            .OnDelete(DeleteBehavior.Cascade); // Защита: удаление диска сотрет его треки
    }
}
