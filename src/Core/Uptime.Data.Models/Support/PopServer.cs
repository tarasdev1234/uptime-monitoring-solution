using Uptime.Data.Models.Branding;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uptime.Data.Models.Support {
    public class PopServer {
        public long Id { get; set; }
        
        public long? DepartmentId { get; set; }

        [JsonIgnore]
        public Department Department { get; set; }

        [NotMapped]
        public string DepartmentName { get; set; }

        public string ServerName { get; set; }

        public int ServerPort { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        [JsonIgnore]
        public string PasswordEncrypted { get; set; }

        [JsonIgnore]
        public string PasswordSalt { get; set; }

        public int UpdateInterval { get; set; }

        [EmailAddress]
        public string EmailAddress { get; set; }

        public bool Enabled { get; set; }

        public EncryptionMethod Encryption { get; set; }
    }
}
