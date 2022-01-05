using System.Collections.Generic;

namespace Reliablesite.Authority.Models.Api
{
    public class RoleDetails
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public List<string> Permissions { get; set; }
    }
}
