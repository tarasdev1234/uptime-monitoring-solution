using System.Collections.Generic;

namespace Uptime.Data.Models.KnowledgeBase {
    public class KbSeries {
        public long Id { get; set; }

        public List<KbArticle> Posts { get; set; }
    }
}
