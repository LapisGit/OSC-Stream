using System.Text.Json.Serialization;

namespace OSC_Stream.Models.Shows;

public class Show
{
    [JsonPropertyName("id")] public int? Id { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("creator")]  public string? Creator { get; set; }
    [JsonPropertyName("bannerImage")]  public string? BannerImage { get; set; }
    [JsonPropertyName("dateReleased")]  public DateTime DateReleased { get; set; } 
    [JsonPropertyName("episodes")]  public List<Episode> Episodes { get; set; } 
}