using AutoMapper.QueryableExtensions;
using Habr.Common.DTO.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Helpers;

public static class PaginationExtensions
{
    public static async Task<PaginatedData<T>> GetPagedDataAsync<T>
        (this IQueryable<T> query, PaginationContext context)
        where T : class
    {
        int totalCount = await query.CountAsync();

        var page = await query
            .Skip(context.PageIndex * context.PageSize)
            .Take(context.PageSize)
            .AsNoTracking()
            .ToListAsync();

        var response = new PaginatedData<T>(page, totalCount, context.PageSize, context.PageIndex + 1);
        return response;
    }
}