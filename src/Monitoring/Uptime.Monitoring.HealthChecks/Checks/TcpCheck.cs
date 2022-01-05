using EnsureThat;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Uptime.Monitoring.HealthChecks.Results;

namespace Uptime.Monitoring.HealthChecks.Checks
{
    public sealed class TcpCheck : BaseHealthCheck<TcpCheckResult>
    {
        private IPAddress? ipAddress;
        private string? dnsName;
        private int port;

        public override string Target
            => $"{dnsName ?? ipAddress?.ToString()}:{port}";

        public TcpCheck(string target, int port)
        {
            EnsureArg.IsNotNull(target, nameof(target));

            if (!IPAddress.TryParse(target, out ipAddress))
            {
                dnsName = target;
            }

            this.port = port;
        }

        protected override async Task<TcpCheckResult> CheckAsyncInternal()
        {
            using var tcpClient = new TcpClient();

            var ip = ipAddress;

            if (ip == null)
            {
                ip = (await Dns.GetHostEntryAsync(dnsName).ConfigureAwait(false)).AddressList[0];
            }

            var stopWatch = Stopwatch.StartNew();
            try
            {
                await tcpClient.ConnectAsync(ip, port).ConfigureAwait(false);
                stopWatch.Stop();
            }
            catch (Exception ex)
            {
                var result = Fail(ex);
                result.ResolvedIp = ip;
                return result;
            }

            return new TcpCheckResult
            {
                CheckStatus = CheckStatus.Healthy,
                ResolvedIp = ip,
                Target = Target,
                ResponseTime = stopWatch.Elapsed
            };
        }
    }
}
