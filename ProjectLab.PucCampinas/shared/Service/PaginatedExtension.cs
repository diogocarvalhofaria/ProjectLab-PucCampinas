using Microsoft.EntityFrameworkCore;
using ProjectLab.PucCampinas.Common.Models;

namespace ProjectLab.PucCampinas.shared.Service;

public static class PaginatedExtension
{
    public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(
        this IQueryable<T> query, int page, int size)
    {
        var totalCount = await query.CountAsync();

        var results = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return new PaginatedResult<T>
        {
            Results = results,
            TotalCount = totalCount,
            CurrentPage = page,
            PageSize = size
        };
    }
}