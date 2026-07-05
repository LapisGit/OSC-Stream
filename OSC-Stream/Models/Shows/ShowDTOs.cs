namespace OSC_Stream.Models.Shows;

public class CreateShowRequest
{
    public string Name { get; set; } = null!;
    public string Creator { get; set; } = null!;
    public string? BannerImage { get; set; }
    public DateTime DateReleased { get; set; }
}

public class CreateEpisodeRequest
{
    public string Name { get; set; } = null!;
    public string VideoId { get; set; } = null!;
    public DateTime DateReleased { get; set; }
}
