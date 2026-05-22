namespace MusicCollection.Domain.Entities;

public class Track
{
    public int Id { get; set; }

    public int Number { get; set; }

    public string Title { get; set; } = null!;

    public TimeSpan Duration { get; set; }

    public int PhysicalDiscId { get; set; }

    public PhysicalDisc PhysicalDisc { get; set; } = null!;
}
