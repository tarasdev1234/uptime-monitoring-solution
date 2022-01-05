using Uptime.Data.Models.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uptime.Data.Models.KnowledgeBase {
    public class KbArticle {
        public long Id { get; set; }

        public long AuthorId { get; set; }

        public ApplicationUser Author { get; set; }

        [Required]
        public string Title { get; set; }

        public DateTime DateCreated { get; set; }

        public string Excerpt { get; set; }

        [Required]
        public string Slug { get; set; }

        public string Content { get; set; }

        public long? SeriesId { get; set; }

        public KbSeries Series { get; set; }

        public long KbCategoryId { get; set; }

        public KbCategory KbCategory { get; set; }

        public int SeriesOrder { get; set; }

        [JsonIgnore]
        public List<KbComment> Comments { get; set; }

        [JsonIgnore]
        public List<KbArticleTag> PostTags { get; set; }

        public List<KbArticleBrand> PostBrands { get; set; }

        public bool IsPublished { get; set; } = false;

        public bool ShowInAll { get; set; } = false;

        [NotMapped]
        public string AuthorName { get; set; }

        [NotMapped]
        public string KbCategoryName { get; set; }

        [NotMapped]
        public int CommentsCount { get; set; }

        [NotMapped]
        public List<long> Brands { get; set; }

        [NotMapped]
        public List<long> Tags { get; set; }

        [NotMapped]
        public List<string> TagNames { get; set; }
    }
}
