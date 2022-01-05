using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uptime.Monitoring.HealthChecks;
using Uptime.Monitoring.HealthChecks.Abstractions;
using Uptime.Monitoring.HealthChecks.Results;
using Uptime.Monitoring.Model.Models;
using DetailNames = Uptime.Monitoring.Model.Models.MonitoringEventDetails.Names;

namespace Uptime.Monitoring.Logic.Extensions
{
    internal static class HealthCheckResultExtensions
    {
        public static Task<MonitoringEvent> ToMonitoringEventAsync(this IHealthCheckResult checkResult, MonitorTask monitorTask)
        {
            return checkResult switch
            {
                TcpCheckResult typedResult => ToMonitoringEventInternal(typedResult, monitorTask),
                HttpCheckResult typedResult => ToMonitoringEventInternal(typedResult, monitorTask),
                PingCheckResult typedResult => ToMonitoringEventInternal(typedResult, monitorTask),
                _ => throw new ArgumentException($"Unexpected check result type '{checkResult?.GetType()?.Name}'"),
            };
        }

        private static Task<MonitoringEvent> ToMonitoringEventInternal(TcpCheckResult checkResult, MonitorTask monitorTask)
        {
            var result = CreateMonitoringEvent(checkResult, monitorTask);

            if (result.Type == EventType.Down)
            {
                result.Details
                    .AddBaseFields(checkResult)
                    .AddNotNull("ResolvedIp", checkResult.ResolvedIp);
            }

            return Task.FromResult(result);
        }

        private static async Task<MonitoringEvent> ToMonitoringEventInternal(HttpCheckResult checkResult, MonitorTask monitorTask)
        {
            var result = CreateMonitoringEvent(checkResult, monitorTask);

            if (result.Type == EventType.Down)
            {
                result.Details.AddBaseFields(checkResult);

                if (checkResult.Response != null)
                {
                    var response = checkResult.Response;
                    var body = await response.Content?.ReadAsStringAsync();

                    result.Details
                        .AddNotNull("StatusCode", (int)response.StatusCode)
                        .AddNotNull("Headers", response.Headers)
                        .AddNotNull("TrailingHeaders", response.TrailingHeaders)
                        .AddNotNull("Body", body);
                }
            }

            return result;
        }

        private static Task<MonitoringEvent> ToMonitoringEventInternal(PingCheckResult checkResult, MonitorTask monitorTask)
        {
            var result = CreateMonitoringEvent(checkResult, monitorTask);

            if (result.Type == EventType.Down)
            {
                result.Details
                    .AddBaseFields(checkResult)
                    .AddNotNull("ResolvedIp", checkResult.ResolvedIp);

                if (checkResult.TraceRoute.Count != 0)
                {
                    var trace = string.Join(";", checkResult.TraceRoute.Select(x => x.ToString()));
                    result.Details.Add("TraceRoute", trace);
                }
            }

            return Task.FromResult(result);
        }

        private static MonitoringEvent CreateMonitoringEvent(HealthCheckResult checkResult, MonitorTask monitorTask)
        {
            var result = new MonitoringEvent
            {
                Type = checkResult.CheckStatus == CheckStatus.Healthy ? EventType.Up : EventType.Down,
                MonitorId = monitorTask.MonitorId,
                UserId = monitorTask.UserId,
                SourceEventId = monitorTask.EventIdToConfirm
            };

            result.Details.AddNotNull(DetailNames.ResponseTime, checkResult.ResponseTime);

            return result;
        }

        private static Dictionary<string, string> AddBaseFields(this Dictionary<string, string> dict, HealthCheckResult checkResult)
        {
            return dict
                .AddNotNull(DetailNames.Description, checkResult.Description)
                .AddNotNull(DetailNames.Exception, checkResult.Exception?.GetType()?.FullName)
                .AddNotNull(DetailNames.Host, checkResult.Target);
        }

        private static Dictionary<string, string> AddNotNull<TValue>(this Dictionary<string, string> dict, string key, TValue value)
        {
            if (value != null)
            {
                dict.Add(key, value.ToString() ?? string.Empty);
            }

            return dict;
        }
    }
}
