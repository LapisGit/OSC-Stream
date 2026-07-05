namespace OSC_Stream.Models.Accounts;

public class CreateAccountRequest
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? ProfileImage { get; set; }
    public DateTime Birthday { get; set; }
}

public class LoginRequest
{
    public string UsernameOrEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
}