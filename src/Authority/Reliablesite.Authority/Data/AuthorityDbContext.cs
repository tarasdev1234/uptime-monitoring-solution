using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Reliablesite.Authority.Models;

namespace Reliablesite.Authority.Data
{
    public class AuthorityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long>
    {
        public AuthorityDbContext(DbContextOptions<AuthorityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
