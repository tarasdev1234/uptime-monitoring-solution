using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Api.ViewModels.Kb {
    public class KbArticleViewModel {
        public List<long> Brands { get; set; }

        [Required]
        public string Title { get; set; }

        public string Content { get; set; }

        public long KbCategoryId { get; set; }

        public long AuthorId { get; set; }

        public bool IsPublished { get; set; }

        public bool ShowInAllBrands { get; set; } = false;
    }
}
