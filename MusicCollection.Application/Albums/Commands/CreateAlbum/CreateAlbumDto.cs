#pragma warning disable SA1313

using MusicCollection.Domain.Entities;

namespace MusicCollection.Application.Albums.Commands.CreateAlbum;

public record CreateAlbumCommand(
    string Title,
    int ReleaseYear,
    string? CatalogNumber,
    string? Label,
    Format Packaging,
    int ArtistId,
    byte[]? CoverImage,
    List<CreateDiscDto> Discs
);

public record CreateDiscDto(
    int DiscNumber,
    string? DiscName,
    List<CreateTrackDto> Tracks
);

public record CreateTrackDto(
    int Number,
    string Title,
    TimeSpan Duration);