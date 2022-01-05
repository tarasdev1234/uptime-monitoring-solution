using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uptime.Data.Models.KnowledgeBase {
    public class KbCategory {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public long? ParentCategoryId { get; set; }

        [JsonIgnore]
        public List<KbArticle> Articles { get; set; }

        [JsonIgnore]
        public KbCategory ParentCategory { get; set; }

        [NotMapped]
        public string ParentName { get; set; }

        [NotMapped]
        public int ArticlesCount { get; set; }
    }
}
