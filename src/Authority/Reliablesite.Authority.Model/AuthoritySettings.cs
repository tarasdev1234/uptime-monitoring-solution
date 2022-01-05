using System;

namespace Reliablesite.Authority.Model
{
    public sealed class AuthoritySettings
    {
        public Uri Url { get; set; }

        public string LogoutPath => Url.AbsolutePath + "/account/logout";

        public string AccountManagementPath => Url.AbsolutePath + "/account/manage";
    }
}
