using WuyiMusic_DAL.Models;

public class Genre
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string? Description { get; set; }=string.Empty;
    // Navigation property
    public virtual ICollection<TrackGenre>? TrackGenres { get; set; }
}