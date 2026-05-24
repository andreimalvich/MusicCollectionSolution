using Microsoft.EntityFrameworkCore;
using MusicCollection.Application.Common.Interfaces;
using MusicCollection.Domain.Entities;

namespace MusicCollection.Application.Tracks.Commands.AddTracksBatch;

public class AddTracksBatchCommandService(IApplicationDbContext context) : IAddTracksBatchCommandService
{
    public async Task ExecuteAsync(AddTracksBatchCommand command, CancellationToken ct = default)
    {
        // Проверяем, существует ли вообще этот диск в БД
        var discExists = await context.Discs.AnyAsync(d => d.Id == command.PhysicalDiscId, ct);
        if (!discExists)
        {
            throw new KeyNotFoundException("Физический диск не найден в базе данных.");
        }

        // Создаем коллекцию доменных сущностей
        var newTracks = command.Tracks.Select(t => new Track
        {
            PhysicalDiscId = command.PhysicalDiscId,
            Number = t.Number,
            Title = t.Title.Trim(),
            Duration = t.Duration,
        }).ToList();

        // Используем встроенную оптимизацию EF Core для массовой вставки
        await context.Tracks.AddRangeAsync(newTracks, ct);

        // Сохраняем всё одним транзакционным SQL-пакетом
        await context.SaveChangesAsync(ct);
    }
}
