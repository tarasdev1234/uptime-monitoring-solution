using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uptime.Notifications.Model.Models;

namespace Uptime.Notifications.Model.Abstractions
{
    public interface ITemplatesService
    {
        IQueryable<Template> FindTemplates(TemplateQuery query);
        Task<Template> GetTemplateAsync(Guid id, CancellationToken token);
        Task<Template> GetTemplateAsync(string scope, string name, CancellationToken token);
        Task AddTemplateAsync(Template template, CancellationToken token);
    }
}
