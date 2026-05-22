namespace MusicCollection.Domain.Entities;

public class PhysicalDisc
{
    public int Id { get; set; }

    public int DiscNumber { get; set; } = 1; // Например: Диск 1, Диск 2

    public string? DiscName { get; set; } // Например: "Live Bonus Tracks"

    public int AlbumId { get; set; }

    public Album Album { get; set; } = null!;

    public List<Track> Tracks { get; set; } = [];
}
