using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Pagination.PagedResult", Version = "1.0")]

namespace Accelerators.Application.Common.Pagination
{
    public class PagedResult<T>
    {
        public PagedResult()
        {
            Data = null!;
        }

        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public IEnumerable<T> Data { get; set; }

        public static PagedResult<T> Create(int totalCount, int pageCount, int pageSize, int pageNumber, IEnumerable<T> data)
        {
            return new PagedResult<T>
            {
                TotalCount = totalCount,
                PageCount = pageCount,
                PageSize = pageSize,
                PageNumber = pageNumber,
                Data = data,
            };
        }
    }
}