using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace OSC_Stream.Services.Infra;

public class JwtTokenService
{
    private static readonly RSA _rsa = RSA.Create(2048);
    private readonly RsaSecurityKey _securityKey;
    private readonly SigningCredentials _signingCredentials;
    private const string KeyId = "7C2F041398671515B0862CB23FAF95B03";

    public JwtTokenService()
    {
        var rsa = Signatures.GetRsaInstance()
                  ?? throw new InvalidOperationException("jwt - sigs service was not found");

        _securityKey = new RsaSecurityKey(rsa) { KeyId = KeyId };

        _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.RsaSha256);
    }

    public string GenerateToken(string accountId, ConfigService config, string platform = "PC", List<string>? additionalRoles = null)
    {
        var now = DateTime.UtcNow;
        var exp = now.AddHours(1);

        var claims = new List<Claim>
        {
            new Claim("iss", "https://lapis.codes"),
            new Claim("nbf", ((DateTimeOffset)now).ToUnixTimeSeconds().ToString()),
            new Claim("iat", ((DateTimeOffset)now).ToUnixTimeSeconds().ToString()),
            new Claim("exp", ((DateTimeOffset)exp).ToUnixTimeSeconds().ToString()),
            new Claim("aud", "https://lapis.codes/resources"),
            new Claim("client_id", "osc-stream"),
            new Claim("sub", accountId),
            new Claim("auth_time", ((DateTimeOffset)now).ToUnixTimeSeconds().ToString()),
            new Claim("idp", "local"),
            new Claim("role", "client"),
            new Claim("jti", Guid.NewGuid().ToString("N")[..32].ToUpper())
        };

        if (config.Config.AdminAccountIds.Contains(accountId))
        {
            claims.Add(new Claim("role", "admin"));
        }
        
        if (additionalRoles != null)
        {
            foreach (var role in additionalRoles)
                claims.Add(new Claim("role", role));
        }

        claims.Add(new Claim("scope", "profile"));

        var identity = new ClaimsIdentity(claims, "jwt");

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateJwtSecurityToken(
            issuer: "https://auth.lapis.codes",
            audience: "https://auth.lapis.codes/resources",
            subject: identity,
            notBefore: now,
            issuedAt: now,
            expires: exp,
            signingCredentials: _signingCredentials);

        return handler.WriteToken(token);
    }

    public string? ValidateAndGetAccountId(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            handler.InboundClaimTypeMap.Clear();

            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _securityKey,
                ValidateIssuer = true,
                ValidIssuer = "https://auth.lapis.codes",
                ValidateAudience = true,
                ValidAudience = "https://auth.lapis.codes/resources",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = handler.ValidateToken(token, parameters, out SecurityToken validatedToken);

            var accountIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "sub");

            return accountIdClaim?.Value;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public int? GetAccountIdFromContext(HttpContext context)
    {
        try
        {
            var authHeader = context.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return null;

            var token = authHeader.Substring("Bearer ".Length);
            var accountId = ValidateAndGetAccountId(token);

            if (string.IsNullOrEmpty(accountId) || !int.TryParse(accountId, out var id))
                return null;

            return id;
        }
        catch
        {
            return null;
        }
    }
}
