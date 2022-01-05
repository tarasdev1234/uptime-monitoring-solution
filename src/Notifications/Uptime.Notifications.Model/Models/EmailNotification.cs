namespace Uptime.Notifications.Model.Models.Email
{
    public class EmailNotification
    {
        public string Body { get; set; }
        
        public string Recipient { get; set; }
        
        public string Subject { get; set; }
        
        public string From { get; set; }
    }
}