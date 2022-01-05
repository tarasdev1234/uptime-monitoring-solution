namespace Uptime.Data.Models.KnowledgeBase {
    public class KbArticleTag {
        public long ArticleId { get; set; }

        public KbArticle Article { get; set; }
        
        public long TagId { get; set; }

        public KbTag Tag { get; set; }
    }
}
