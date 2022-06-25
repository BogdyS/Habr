using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO.User;
using Microsoft.IdentityModel.Tokens;

namespace Habr.WebAPI;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetJwt(object userObject)
    {
        var user = (UserDTO)userObject;

        var claims = new[] {new Claim(nameof(ClaimTypes.NameIdentifier), user.Id.ToString())};
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        int lifetimeAccessTokenInHours = int.Parse(_configuration["Jwt:AccessLifetimeInHours"]);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(lifetimeAccessTokenInHours),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GetRefreshToken()
    {
        byte[] randomNumber = new byte[64];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public DateTime RefreshTokenValidTo => DateTime.UtcNow.AddDays(int.Parse(_configuration["Jwt:RefreshLifetimeInDays"]));
}