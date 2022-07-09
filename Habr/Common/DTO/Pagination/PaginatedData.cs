namespace Habr.Common.DTO.Pagination;

public class PaginatedData<T> where T : class
{
    public PaginatedData(IEnumerable<T> list, int totalCount, int pageSize, int pageNumber)
    {
        TotalCount = totalCount;
        PageSize = pageSize;
        PageNumber = pageNumber;
        Data = list.ToList();
    }

    public List<T> Data { get; set; }
    public int TotalCount { get; private set; }
    public int PageSize { get; private set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public int PageNumber { get; private set; }
}