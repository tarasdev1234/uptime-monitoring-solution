using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Reliablesite.Service.Model.Dto
{
    public class PagedCollection<T>
    {
        public int PageIndex { get; private set; }

        public int PageSize { get; private set; }

        [JsonPropertyName("count")]
        public long TotalCount { get; private set; }

        public ICollection<T> Data { get; private set; }

        public PagedCollection(ICollection<T> data, int pageIndex, int pageSize, long totalCount)
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
    }
}
