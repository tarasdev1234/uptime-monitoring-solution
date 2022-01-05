using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Uptime.Notifications.Email.Abstractions;
using Uptime.Notifications.Model.Models;
using Uptime.Notifications.Model.Settings;

namespace Uptime.Notifications.Email.Services
{
    internal sealed class SmtpNotificationSender : ISmtpNotificationSender
    {
        private readonly SmtpSettings settings;
        private readonly ILogger<SmtpNotificationSender> log;

        public SmtpNotificationSender(IOptions<SmtpSettings> options, ILogger<SmtpNotificationSender> log)
        {
            settings = options.Value;
            this.log = log;
        }

        public async Task SendAsync(Notification notification, string renderedNotification, CancellationToken token)
        {
            log.LogDebug("Sending email with notification {NotificationScope}.{NotificationType} to {N} recepients", notification.Scope, notification.Type, notification.Destinations.Count);

            var message = new MailMessage
            {
                Subject = GetSubject(notification),
                Body = renderedNotification,
                IsBodyHtml = true,
                From = new MailAddress(GetFrom())
            };
            message.To.Add(string.Join(",", notification.Destinations));

            using (var client = new SmtpClient(settings.Host, settings.Port))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(settings.UserName, settings.Password);
                token.ThrowIfCancellationRequested();
                await client.SendMailAsync(message);
            }
        }

        public string GetSubject(Notification notification)
        {
            return "Uptime.Engineer";
        }

        public string GetFrom()
        {
            return "noreply@uptime.engineer";
        }
    }
}