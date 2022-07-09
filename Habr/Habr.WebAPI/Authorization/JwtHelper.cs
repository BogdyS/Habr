using System.Security.Claims;
using Habr.Common;
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

    public static int GetClaimUserId(IEnumerable<Claim> claims)
    {
        return int.Parse(claims.FirstOrDefault(claim => claim.Type == nameof(ClaimTypes.NameIdentifier))?.Value!);
    }

    public static RolesEnum GetClaimRole(IEnumerable<Claim> claims)
    {
        return Enum.Parse<RolesEnum>(claims.FirstOrDefault(claim => claim.Type == nameof(ClaimTypes.Role))?.Value!);
    }
}