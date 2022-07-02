namespace Habr.Common.DTO.Pagination;

public class PaginatedDTO<T> where T : class
{
    public List<T> Paginated { get; set; }
    public int TotalCount { get; private set; }
    public int PageSize { get; private set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public int PageNumber { get; private set; }

    private PaginatedDTO(IEnumerable<T> list, int totalCount, int pageSize, int pageNumber)
    {
        TotalCount = totalCount;
        PageSize = pageSize;
        PageNumber = pageNumber;
        Paginated = list.ToList();
    }

    public static PaginatedDTO<T> ToPaginatedDTO(IQueryable<T> query, int pageSize, int pageNumber)
    {
        var list = query.Skip(pageNumber * pageSize).Take(pageSize).ToList();

        return new PaginatedDTO<T>(list, query.Count(), pageSize, pageNumber);
    }
}