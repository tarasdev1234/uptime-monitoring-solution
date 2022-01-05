using System;

namespace Uptime.Notifications.Model.Models
{
    public class Template
    {
        public Guid Id { get; set; }
        public string Scope { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public TemplateEngineType RenderEngine { get; set; }
    }
}
