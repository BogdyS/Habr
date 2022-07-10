using AutoMapper;

namespace Habr.Common.DTO.Pagination;

public class PaginationContext
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}