using Uptime.Notifications.Model.Models;

namespace Uptime.Notifications.Model.Abstractions
{
    public interface ITemplateEngineProvider
    {
        ITemplateEngine GetTemplateEngine(TemplateEngineType engine);
    }
}
