using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OSC_Stream.Data;
using OSC_Stream.Models.Accounts;
using OSC_Stream.Services.Infra;

namespace OSC_Stream.Services.Controllers;

[ApiController]
[Route("/api/accounts")]
public class AccountsController : ControllerBase
{
    [HttpPost("create")]
    public async Task<IResult> CreateAccount([FromBody] CreateAccountRequest request, [FromServices] AppDbContext db, [FromServices] JwtTokenService jwt, [FromServices] ConfigService config)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.BadRequest(new
            {
                error = "Missing required fields."
            });
        }

        bool usernameExists = await db.Accounts
            .AnyAsync(a => a.Username == request.Username);

        if (usernameExists)
        {
            return Results.BadRequest(new
            {
                error = "Username already exists."
            });
        }

        bool emailExists = await db.Accounts
            .AnyAsync(a => a.Email == request.Email);

        if (emailExists)
        {
            return Results.BadRequest(new
            {
                error = "Email already exists."
            });
        }

        Account account = new()
        {
            Username = request.Username,
            DisplayName = request.DisplayName ?? request.Username,
            Email = request.Email,
            Birthday = request.Birthday,
            ProfileImage = request.ProfileImage,
            CreatedAt = DateTime.UtcNow,

            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),

            Roles = new List<string> { "user" }
        };

        db.Accounts.Add(account);
        await db.SaveChangesAsync();

        string token = jwt.GenerateToken(
            account.AccountId.ToString(),
            config,
            additionalRoles: account.Roles
        );

        return Results.Ok(new
        {
            token,
            account = new
            {
                account.AccountId,
                account.Username,
                account.DisplayName,
                account.Email,
                account.ProfileImage,
                account.CreatedAt,
                account.Roles
            }
        });
    }

    [HttpPost("login")]
    public async Task<IResult> Login([FromBody] LoginRequest request, [FromServices] AppDbContext db, [FromServices] JwtTokenService jwt, [FromServices] ConfigService config)
    {
        Account? account = await db.Accounts.FirstOrDefaultAsync(a =>
            a.Username == request.UsernameOrEmail ||
            a.Email == request.UsernameOrEmail);

        if (account == null)
        {
            return Results.Unauthorized();
        }

        bool validPassword = BCrypt.Net.BCrypt.Verify(
            request.Password,
            account.Password);

        if (!validPassword)
        {
            return Results.Unauthorized();
        }

        string token = jwt.GenerateToken(
            account.AccountId.ToString(),
            config,
            additionalRoles: account.Roles
        );

        return Results.Ok(new
        {
            token,
            account = new
            {
                account.AccountId,
                account.Username,
                account.DisplayName,
                account.Email,
                account.ProfileImage,
                account.CreatedAt,
                account.Roles
            }
        });
    }
}