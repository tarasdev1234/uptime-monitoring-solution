using EnsureThat;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Uptime.Monitoring.HealthChecks.Results;

namespace Uptime.Monitoring.HealthChecks.Checks
{
    public sealed class PingCheck : BaseHealthCheck<PingCheckResult>
    {
        private const int timeoutMs = 1000;
        private const int ttl = 30;

        private static byte[] payload => Encoding.ASCII.GetBytes("Uptime.Monitoring");

        private string host;

        public PingCheck (string host)
        {
            this.host = EnsureArg.IsNotNull(host, nameof(host));
        }

        public override string Target => host;

        protected override async Task<PingCheckResult> CheckAsyncInternal ()
        {
            using var pingSender = new Ping();

            var options = new PingOptions(ttl, true);

            var pingResult = await pingSender.SendPingAsync(host, timeoutMs, payload, options).ConfigureAwait(false);

            if (pingResult.Status == IPStatus.Success)
            {
                return new PingCheckResult()
                {
                    CheckStatus = CheckStatus.Healthy,
                    Description = GetStatusDescription(pingResult.Status),
                    ResponseTime = TimeSpan.FromMilliseconds(pingResult.RoundtripTime),
                    Target = host,
                    ResolvedIp = pingResult.Address
                };
            }
            else
            {
                var trace = new List<IPAddress>();

                for (int traceTtl = 1; traceTtl < ttl; ++traceTtl)
                {
                    options = new PingOptions(traceTtl, true);

                    var traceResult = await pingSender.SendPingAsync(host, timeoutMs, payload, options).ConfigureAwait(false);

                    if (traceResult.Status == IPStatus.TtlExpired)
                    {
                        trace.Add(traceResult.Address);
                    }
                    else if (traceResult.Status == IPStatus.Success)
                    {
                        trace.Add(traceResult.Address);
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                return new PingCheckResult()
                {
                    CheckStatus = CheckStatus.Unhealthy,
                    Description = GetStatusDescription(pingResult.Status),
                    ResponseTime = TimeSpan.FromMilliseconds(pingResult.RoundtripTime),
                    Target = host,
                    ResolvedIp = pingResult.Address,
                    TraceRoute = trace
                };
            }
        }

        public static string GetStatusDescription(IPStatus status) =>
            status switch
            {
                IPStatus.BadDestination => "Bad Destination",
                IPStatus.BadHeader => "Bad Header",
                IPStatus.BadOption => "Bad Option",
                IPStatus.BadRoute => "Bad Route",
                IPStatus.DestinationHostUnreachable => "Destination Host Unreachable",
                IPStatus.DestinationNetworkUnreachable => "Destination Network Unreachable",
                IPStatus.DestinationPortUnreachable => "Destination Port Unreachable",
                IPStatus.DestinationProhibited => "Destination Prohibited",
                IPStatus.DestinationScopeMismatch => "Destination Scope Mismatch",
                IPStatus.DestinationUnreachable => "Destination Unreachable",
                IPStatus.HardwareError => "Hardware Error",
                IPStatus.IcmpError => "Icmp Error",
                IPStatus.NoResources => "No Resources",
                IPStatus.PacketTooBig => "Packet Too Big",
                IPStatus.ParameterProblem => "Parameter Problem",
                IPStatus.SourceQuench => "Source Quench",
                IPStatus.Success => "Success",
                IPStatus.TimedOut => "Timed Out",
                IPStatus.TimeExceeded => "Time Exceeded",
                IPStatus.TtlExpired => "Ttl Expired",
                IPStatus.TtlReassemblyTimeExceeded => "Ttl Reassembly Time Exceeded",
                IPStatus.Unknown => "Unknown",
                IPStatus.UnrecognizedNextHeader => "Unrecognized Next Header",
                _ => status.ToString(),
            };
    }
}
