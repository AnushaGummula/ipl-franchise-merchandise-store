using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IPL_Franchises.Application.DTOs.Auth;
using IPL_Franchises.Infrastructure.Identity;
using Microsoft.IdentityModel.Tokens;

namespace IPL_Franchises.API.Services;

public class JwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public AuthResponse CreateToken(
        ApplicationUser user)
    {
        var key =
            _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "JWT key is not configured.");

        var issuer =
            _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "JWT issuer is not configured.");

        var audience =
            _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "JWT audience is not configured.");

        var expiryMinutes =
            int.TryParse(
                _configuration["Jwt:ExpiryMinutes"],
                out var minutes)
                ? minutes
                : 120;


        var expiresAt =
            DateTime.UtcNow.AddMinutes(
                expiryMinutes);


        var claims = new List<Claim>
        {
            new(
                ClaimTypes.NameIdentifier,
                user.Id),

            new(
                JwtRegisteredClaimNames.Sub,
                user.Id),

            new(
                ClaimTypes.Name,
                user.FullName),

            new(
                ClaimTypes.Email,
                user.Email ?? string.Empty),

            new(
                JwtRegisteredClaimNames.Email,
                user.Email ?? string.Empty),

            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };


        var securityKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));


        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);


        var token =
            new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);


        return new AuthResponse
        {
            Token =
                new JwtSecurityTokenHandler()
                    .WriteToken(token),

            ExpiresAt = expiresAt,

            UserId = user.Id,

            FullName = user.FullName,

            Email =
                user.Email ?? string.Empty
        };
    }
}
