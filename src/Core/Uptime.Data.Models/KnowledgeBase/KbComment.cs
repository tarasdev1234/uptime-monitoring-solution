using Uptime.Data.Models.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Uptime.Data.Models.KnowledgeBase {
    public class KbComment {
        public long Id { get; set; }

        public long ArticleId { get; set; }

        public KbArticle Article { get; set; }

        public ApplicationUser Author { get; set; }

        public List<KbComment> Replies { get; set; }

        public KbComment Parent { get; set; }
        
        public DateTime Date { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
