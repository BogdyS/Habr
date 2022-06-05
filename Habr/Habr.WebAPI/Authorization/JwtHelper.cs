using System.Security.Claims;

namespace Habr.WebAPI;

public static class JwtHelper
{
    public static bool IsJwtIdClaimValid(IEnumerable<Claim> claims, int userId)
    {
        return userId.ToString() == 
               claims.FirstOrDefault(claim => 
                   claim.Type == nameof(ClaimTypes.NameIdentifier))?.Value;
    }
}