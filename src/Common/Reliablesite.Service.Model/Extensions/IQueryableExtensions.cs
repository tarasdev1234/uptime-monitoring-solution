using Reliablesite.Service.Model.Dto;

namespace System.Linq
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paged<T>(this IQueryable<T> query, PagedQuery p)
        {
            return query.Skip(p.PageSize.Value * (p.PageIndex.Value - 1))
                        .Take(p.PageSize.Value);
        }
    }
}
