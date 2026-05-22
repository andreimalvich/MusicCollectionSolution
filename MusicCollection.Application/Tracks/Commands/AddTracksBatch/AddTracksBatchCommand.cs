#pragma warning disable SA1313

namespace MusicCollection.Application.Tracks.Commands.AddTracksBatch;

public record AddTracksBatchCommand(
    int PhysicalDiscId,
    IReadOnlyCollection<TrackImportItemDto> Tracks
);

public record TrackImportItemDto(
    int Number,
    string Title,
    TimeSpan Duration
);
