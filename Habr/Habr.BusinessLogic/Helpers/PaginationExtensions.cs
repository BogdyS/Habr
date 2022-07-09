using AutoMapper.QueryableExtensions;
using Habr.Common.DTO.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Helpers;

public static class PaginationExtensions
{
    public static async Task<PaginatedData<TDestination>> GetPagedDataAsync<TSource, TDestination>(this IQueryable<TSource> query, PaginationContext context)
        where TDestination : class
        where TSource : class
    {
        int totalCount = await query.CountAsync();

        var page = await query
            .Skip(context.PageIndex * context.PageSize)
            .Take(context.PageSize)
            .ProjectTo<TDestination>(context.Provider)
            .AsNoTracking()
            .ToListAsync();

        var response = new PaginatedData<TDestination>(page, totalCount, context.PageSize, context.PageIndex + 1);
        return response;
    }
}