namespace MusicCollection.Application.Tracks.Commands.AddTracksBatch;

public interface IAddTracksBatchCommandService
{
    Task ExecuteAsync(AddTracksBatchCommand command, CancellationToken ct = default);
}