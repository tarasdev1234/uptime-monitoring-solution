using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Uptime.Data;
using Uptime.Data.Models.Branding;

namespace Uptime.Mvc.Controllers {
    public class BaseController : Controller {
        private Lazy<Brand> brand;

        protected string CurrentHost {
            get {
                return Request.Host.Host;
            }
        }

        protected Brand CurrentBrand {
            get {
                return brand.Value;
            }
        }

        protected ApplicationDbContext dbContext;
        
        public BaseController (ApplicationDbContext ctx) {
            dbContext = ctx;
            brand = new Lazy<Brand>(() => {
                return dbContext.Brands
                        .Where(b => b.Url == CurrentHost)
                        .FirstOrDefault();
            });
        }
    }
}
