using System.Security.Claims;
using Habr.Common;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebAPI.Controllers
{
    public abstract class HabrController : ControllerBase
    {
        protected RolesEnum GetUserRole(IEnumerable<Claim> claims, int userId)
        {
            JwtHelper.IsJwtIdClaimValid(claims, userId);

            return JwtHelper.GetClaimRole(claims);
        }

        protected void CheckClaimId(IEnumerable<Claim> claims, int userId)
        {
            JwtHelper.IsJwtIdClaimValid(claims, userId);
        }
    }
}
