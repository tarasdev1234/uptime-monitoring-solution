using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Uptime.Data;
using Uptime.Mvc.Controllers;
using Users.Security;

namespace Admin.Api.Controllers.Identity {
    [Route("api/permissions")]
    [Authorize(Roles = "Admin")]
    public class PermissionController : BaseController {
        private readonly IEnumerable<IPermissionProvider> registeredPermissions;

        public PermissionController (ApplicationDbContext context, IEnumerable<IPermissionProvider> prms) : base(context) {
            registeredPermissions = prms;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Permission>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll () {
            var permissions = new List<Permission>();
            var grouped = registeredPermissions
                          .SelectMany(prvdr => prvdr.GetPermissions())
                          .GroupBy(key => key.Type)
                          .Select(grp => new { type = grp.Key, permissions = grp.ToList() })
                          .ToList();
            
            return Ok(grouped);
        }

        //[HttpGet]
        //[ProducesResponseType(typeof(List<SecureObject>), (int)HttpStatusCode.OK)]
        //public async Task<IActionResult> GetAll () {
        //    if (!ModelState.IsValid) {
        //        return BadRequest(ModelState);
        //    }

        //    var query = dbContext.SecureObjects
        //                .Include(so => so.Permissions);

        //    var total = await query.LongCountAsync();

        //    var items = query
        //        .OrderBy(c => c.Id)
        //        .ToList();

        //    return Ok(items);
        //}
    }
}
