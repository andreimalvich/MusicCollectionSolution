#pragma warning disable SA1313

namespace MusicCollection.Application.Albums.Queries.GetAlbumsCarousel;

public record AlbumCarouselItemDto(
    int Id,
    string Title,
    string ArtistName,
    int ReleaseYear,
    byte[]? CoverImageData // Массив байт для рендеринга картинки на витрине
);
