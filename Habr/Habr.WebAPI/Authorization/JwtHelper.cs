using System.Security.Claims;
using Habr.Common.Exceptions;

namespace Habr.WebAPI;

public static class JwtHelper
{
    public static void IsJwtIdClaimValid(IEnumerable<Claim> claims, int userId)
    {
        if (userId.ToString() !=
            claims.FirstOrDefault(claim => claim.Type == nameof(ClaimTypes.NameIdentifier))?.Value)
        {
            throw new ForbiddenException("Forbidden");
        }
    }
}