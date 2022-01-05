using Admin.Api.ViewModels.Brands;
using Admin.Api.ViewModels.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uptime.Data;
using Uptime.Data.Models.Branding;

namespace Admin.Api.Controllers.Controllers
{
    [Route("api/users")]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase {
        private readonly ApplicationDbContext dbContext;

        public UserController (ApplicationDbContext context) {
            dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        [Route("{userId:long}/company")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserCompanyInfo(long userId)
        {
            var companySettings = await dbContext.CompanySettings.FirstOrDefaultAsync(x => x.UserId == userId);
            var departments = await dbContext.UserDepartments.Where(x => x.UserId == userId).Select(x => x.DepartmentId).ToListAsync();

            var response = new CompanyInfo
            {
                Company = new CompanySettingsViewModel
                {
                    CompanyId = companySettings?.CompanyId,
                    IsAdmin = companySettings?.IsAdmin,
                    IsOwner = companySettings?.IsOwner
                },
                Departments = departments
            };

            return Ok(response);
        }

        [Route("{userId:long}/company")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserCompanyInfo(long userId, [FromBody]CompanyInfo companyInfo)
        {
            var newCompany = companyInfo?.Company;
            var companySettings = await dbContext.CompanySettings.FirstOrDefaultAsync(x => x.UserId == userId);

            if (newCompany != null && newCompany.CompanyId != null)
            {
                if (companySettings == null)
                {
                    companySettings = new CompanySettings();
                    dbContext.CompanySettings.Add(companySettings);
                }

                companySettings.UserId = userId;
                companySettings.CompanyId = newCompany.CompanyId.Value;
                companySettings.IsAdmin = newCompany.IsAdmin ?? false;
                companySettings.IsOwner = newCompany.IsOwner ?? false;
            }
            else
            {
                dbContext.CompanySettings.Remove(companySettings);
            }

            var newDepartments = companyInfo?.Departments ?? new List<long>();
            var departments = await dbContext.UserDepartments.Where(x => x.UserId == userId).ToListAsync();

            var departmentsToAdd = newDepartments
                .Except(departments.Select(x => x.DepartmentId).ToList())
                .Select(x => new UserDepartment
                {
                    DepartmentId = x,
                    UserId = userId
                });

            var departmentsToDelete = departments
                .Where(x => !newDepartments.Contains(x.DepartmentId));

            dbContext.UserDepartments.RemoveRange(departmentsToDelete);
            dbContext.UserDepartments.AddRange(departmentsToAdd);

            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
