using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO.User;
using Microsoft.IdentityModel.Tokens;

namespace Habr.WebAPI;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    public JwtService()
    {
        _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    }
    public string GetJwt(object userObject)
    {
        var user = (UserDTO)userObject;

        var claims = new[] {new Claim(nameof(ClaimTypes.NameIdentifier), user.Id.ToString())};
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}