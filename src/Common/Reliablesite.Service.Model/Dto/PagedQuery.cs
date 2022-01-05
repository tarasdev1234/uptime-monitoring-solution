using System.ComponentModel.DataAnnotations;

namespace Reliablesite.Service.Model.Dto
{
    public class PagedQuery
    {
        [Range(1, 50)]
        public int? PageSize { get; set; } = 10;

        [Range(1, int.MaxValue)]
        public int? PageIndex { get; set; } = 1;
    }
}
