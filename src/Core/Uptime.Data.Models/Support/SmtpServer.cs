using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Uptime.Data.Models.Support {
    public class SmtpServer {
        public long Id { get; set; }

        public string Name { get; set; }

        public string ServerName { get; set; }
        
        public int ServerPort { get; set; }

        public string Login { get; set; }
        
        public string Password { get; set; }

        [JsonIgnore]
        public string PasswordEncrypted { get; set; }

        [JsonIgnore]
        public string PasswordSalt { get; set; }

        [EmailAddress]
        public string EmailAddress { get; set; }

        public bool Default { get; set; }

        public EncryptionMethod Encryption { get; set; }
    }
}
