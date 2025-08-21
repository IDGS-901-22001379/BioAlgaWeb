using System.Collections.Generic;

namespace BioAlga.Backend.Dtos
{
    public class PagedResponse<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public IEnumerable<T> Items { get; set; } = new List<T>();
    }
}
