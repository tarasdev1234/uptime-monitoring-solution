using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Uptime.Data.Models.KnowledgeBase {
    public class KbTag {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        [JsonIgnore]
        public List<KbArticleTag> PostTags { get; set; }
    }
}
