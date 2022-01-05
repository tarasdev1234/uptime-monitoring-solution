using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Reliablesite.Service.Core.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reliablesite.Service.Core
{
    internal class ApiRoutesProvider
    {
        private readonly IApiDescriptionGroupCollectionProvider apiDescriptionsProvider;

        public ApiRoutesProvider(IApiDescriptionGroupCollectionProvider apiDescriptionsProvider)
        {
            this.apiDescriptionsProvider = apiDescriptionsProvider;
        }

        public IList<string> GetApiRoutePrefixes()
        {
            var routes = new List<string>();

            var descriptors = apiDescriptionsProvider
                .ApiDescriptionGroups.Items
                .SelectMany(x => x.Items)
                .Where(x => {
                    var controllerDescriptor = x.ActionDescriptor as ControllerActionDescriptor;
                    return controllerDescriptor != null && controllerDescriptor.ControllerTypeInfo != typeof(HealthCheckController);
                })
                .GroupBy(x => ((ControllerActionDescriptor)x.ActionDescriptor).ControllerTypeInfo);

            foreach (var group in descriptors)
            {
                var route = FindCommonPrefix(group.Select(x => x.RelativePath).ToList());

                if (!string.IsNullOrEmpty(route))
                {
                    routes.Add(route);
                }
            }

            return routes;
        }

        private string FindCommonPrefix(IList<string> routes)
        {
            if (routes == null && routes.Count == 0)
            {
                return null;
            }

            var prefix = new string(
                routes.First().Substring(0, routes.Min(s => s.Length))
                .TakeWhile((c, i) => routes.All(s => s[i] == c)).ToArray());

            var braceIdx = prefix.IndexOf('{');
            prefix = braceIdx > -1 ? prefix.Substring(0, braceIdx) : prefix;

            return '/' + prefix.TrimEnd('/');
        }
    }
}
