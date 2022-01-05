using Uptime.Data.Models.Identity;

namespace Uptime.Data.Models.Branding {
    public class UserDepartment {
        public long DepartmentId { get; set; }

        public Department Department { get; set; }
        
        public long UserId { get; set; }
    }
}
