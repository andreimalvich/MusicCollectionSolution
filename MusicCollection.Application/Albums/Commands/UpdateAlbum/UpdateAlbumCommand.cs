#pragma warning disable SA1313

using MusicCollection.Domain.Entities;

namespace MusicCollection.Application.Albums.Commands.UpdateAlbum;

public record UpdateAlbumCommand(
    int Id,
    string Title,
    int ReleaseYear,
    string? CatalogNumber,
    string? Label,
    Format Packaging
);
