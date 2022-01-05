using System;
using Uptime.Notifications.Model.Models;

namespace Uptime.Notifications.Web.Dto
{
    public class TemplateShort
    {
        public Guid Id { get; set; }
        public string Scope { get; set; }
        public string Name { get; set; }
        public TemplateEngineType RenderEngine { get; set; }
    }
}
