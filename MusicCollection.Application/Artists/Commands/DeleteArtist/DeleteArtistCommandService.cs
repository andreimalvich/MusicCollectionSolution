using MusicCollection.Application.Common.Interfaces;

namespace MusicCollection.Application.Artists.Commands.DeleteArtist;

public class DeleteArtistCommandService(IApplicationDbContext context) : IDeleteArtistCommandService
{
    public async Task ExecuteAsync(DeleteArtistCommand command, CancellationToken ct = default)
    {
        var artist = await context.Artists.FindAsync([command.Id], ct)
            ?? throw new KeyNotFoundException($"Артист с ID {command.Id} не найден.");

        context.Artists.Remove(artist); // удаление Artist->Albums->Disks->Tracks

        await context.SaveChangesAsync(ct);
    }
}
