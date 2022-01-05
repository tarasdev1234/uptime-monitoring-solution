using Uptime.Data.Models.KnowledgeBase;
using System.Collections.Generic;
using Newtonsoft.Json;
using Uptime.Data.Models.Support;
using Uptime.Data.Models.Plugins;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uptime.Data.Models.Branding {
    public class Brand {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Theme { get; set; }

        public string Url { get; set; }

        public long? SmtpServerId { get; set; }

        [JsonIgnore]
        public SmtpServer SmtpServer { get; set; }
        
        [JsonIgnore]
        public List<Department> Departments { get; set; }

        [JsonIgnore]
        public List<KbArticleBrand> KbArticles { get; set; }
        
        [NotMapped]
        public List<string> Plugins { get; set; }

        [JsonIgnore]
        public List<BrandPlugin> BrandPlugins { get; set; }
    }
}
