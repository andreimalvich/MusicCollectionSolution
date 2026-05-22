namespace MusicCollection.Domain.Entities;

public class Album
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int ReleaseYear { get; set; }

    public string? CatalogNumber { get; set; }

    public string? Label { get; set; }

    public Format Packaging { get; set; } = Format.JewelCase;

    public int ArtistId { get; set; }

    public Artist Artist { get; set; } = null!;

    public AlbumImage? Image { get; set; }

    public ICollection<PhysicalDisc> PhysicalDiscs { get; set; } = [];
}
