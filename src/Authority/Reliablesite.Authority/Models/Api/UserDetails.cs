using System.Collections.Generic;

namespace Reliablesite.Authority.Models.Api
{
    public class UserDetails
    {
        public bool TwoFactorEnabled { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public string Signature { get; set; }

        public bool EmailConfirmed { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public long Id { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public List<string> Roles { get; set; }

        public List<long> Departments { get; set; }

        public List<string> Permissions { get; set; }
    }
}
