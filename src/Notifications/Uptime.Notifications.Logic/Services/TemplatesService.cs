using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uptime.Notifications.Data;
using Uptime.Notifications.Model.Abstractions;
using Uptime.Notifications.Model.Models;

namespace Uptime.Notifications.Logic.Services
{
    internal sealed class TemplatesService : ITemplatesService
    {
        private readonly UptimeNotificationsContext context;

        public TemplatesService(UptimeNotificationsContext context)
        {
            this.context = context;
        }

        public async Task AddTemplateAsync(Template templateInfo, CancellationToken token)
        {
            context.Templates.Add(templateInfo);
            await context.SaveChangesAsync(token);
        }

        public async Task<Template> GetTemplateAsync(string scope, string name, CancellationToken token)
        {
            return await context.Templates.FirstOrDefaultAsync(x => x.Scope == scope && x.Name == name, token);
        }

        public async Task<Template> GetTemplateAsync(Guid id, CancellationToken token)
        {
            return await context.Templates.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public IQueryable<Template> FindTemplates(TemplateQuery query)
        {
            IQueryable<Template> templates = context.Templates;
            if (query.Scope != null)
            {
                templates = templates.Where(x => x.Scope == query.Scope);
            }

            if (query.Name != null)
            {
                templates = templates.Where(x => x.Name == query.Name);
            }

            return templates;
        }
    }
}
