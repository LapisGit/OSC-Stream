using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OSC_Stream.Models.Shows;

public class Episode
{
    [JsonPropertyName("showId")] public int? ShowId { get; set; }
    [JsonPropertyName("id")] public int? Id { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("videoId")] public string? VideoId { get; set; }
    [NotMapped]
    [JsonPropertyName("bannerImage")]
    public string? BannerImage => VideoId is not null ? $"https://img.youtube.com/vi/{VideoId}/maxresdefault.jpg" : null;
    [JsonPropertyName("dateReleased")] public DateTime DateReleased { get; set; }
}