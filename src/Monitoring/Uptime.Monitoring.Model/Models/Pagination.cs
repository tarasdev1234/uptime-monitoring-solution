namespace Uptime.Monitoring.Model.Models
{
    public class Pagination
    {
        public int PageSize { get; set; }

        public byte[] State { get; set; }

        public static Pagination First => new Pagination { PageSize = 1 };
    }
}
