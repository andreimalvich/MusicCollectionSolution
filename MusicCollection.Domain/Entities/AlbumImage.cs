namespace MusicCollection.Domain.Entities;

public class AlbumImage
{
    public int Id { get; set; }

    public byte[] Data { get; set; } = null!;

    public int AlbumId { get; set; }

    public Album Album { get; set; } = null!;
}
