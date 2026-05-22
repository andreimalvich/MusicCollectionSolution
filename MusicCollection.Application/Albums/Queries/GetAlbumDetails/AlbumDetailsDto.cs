#pragma warning disable SA1313

namespace MusicCollection.Application.Albums.Queries.GetAlbumDetails;

public record AlbumDetailsDto(
    int Id,
    string Title,
    string ArtistName,
    int ReleaseYear,
    string? CatalogNumber,
    string? Label,
    string Packaging,
    byte[]? CoverImageData,
    IReadOnlyCollection<DiscDetailsDto> Discs
);

public record DiscDetailsDto(
    int Id,
    int DiscNumber,
    string? DiscName,
    IReadOnlyCollection<TrackDetailsDto> Tracks
);

public record TrackDetailsDto(
    int Id,
    int Number,
    string Title,
    string Duration // Передаем строку "03:45", чтобы GUI не мучился с форматированием TimeSpan
);
