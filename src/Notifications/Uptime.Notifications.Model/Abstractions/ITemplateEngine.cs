using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Uptime.Notifications.Model.Models;

namespace Uptime.Notifications.Model.Abstractions
{
    public interface ITemplateEngine
    {
        Task<string> RenderAsync(Template template, ExpandoObject data, CancellationToken token);
    }
}
