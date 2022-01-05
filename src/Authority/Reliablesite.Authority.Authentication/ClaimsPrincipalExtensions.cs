using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Reliablesite.Authority.Authentication
{
    public static class ClaimsPrincipalExtensions
    {
        public static long GetId(this ClaimsPrincipal user)
        {
            var raw = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (!long.TryParse(raw, out long id))
            {
                throw new ArgumentException("Can't find claim contains valid user id");
            }

            return id;
        }

        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
        }
    }
}
