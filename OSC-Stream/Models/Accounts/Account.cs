using System.Text.Json.Serialization;

namespace OSC_Stream.Models.Accounts;

public class Account
{
    [JsonPropertyName("accountId")] public int? AccountId { get; set; }
    [JsonPropertyName("profileImage")]  public string? ProfileImage { get; set; }
    [JsonPropertyName("birthday")]  public DateTime Birthday { get; set; } 
    [JsonPropertyName("username")] public string? Username { get; set; }
    [JsonPropertyName("displayName")] public string? DisplayName { get; set; }
    [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; }
    [JsonPropertyName("email")] public string? Email { get; set; }
    [JsonPropertyName("password")] public string? Password { get; set; }
    [JsonPropertyName("roles")] public List<string>? Roles { get; set; }
}