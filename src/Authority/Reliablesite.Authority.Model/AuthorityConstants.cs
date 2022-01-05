namespace Reliablesite.Authority.Model
{
    public static class AuthorityConstants
    {
        /// <summary>
        /// Policy that handles both cookie and jwt bearer authorization
        /// </summary>
        public const string CookieOrJwtPolicy = "cookie+jwt_bearer";

        public const string OpenIdConnectScheme = "oidc";
    }
}
