using Uptime.Data.Models.Branding;
using Newtonsoft.Json;

namespace Uptime.Data.Models.KnowledgeBase {
    public class KbArticleBrand {
        public long ArticleId { get; set; }

        [JsonIgnore]
        public KbArticle Article { get; set; }
        
        public long BrandId { get; set; }

        [JsonIgnore]
        public Brand Brand { get; set; }
    }
}
