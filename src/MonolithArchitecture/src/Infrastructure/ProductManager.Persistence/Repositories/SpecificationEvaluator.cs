using Microsoft.EntityFrameworkCore;
using ProductManager.Domain.Common;

namespace ProductManager.Persistence.Repositories;

/// <summary>
/// Evaluator to apply specification to IQueryable
/// This is the core component that translates specifications into EF Core queries
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public static class SpecificationEvaluator<T> where T : class
{
    /// <summary>
    /// Apply specification to queryable and return the modified query
    /// </summary>
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        var query = inputQuery;

        // Apply no tracking if specified (better performance for read-only queries)
        if (specification.IsNoTracking)
        {
            query = query.AsNoTracking();
        }

        // Apply criteria (WHERE clause)
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // Apply includes (eager loading)
        query = specification.Includes
            .Aggregate(query, (current, include) => current.Include(include));

        // Apply string-based includes (for nested properties like "Category.Parent")
        query = specification.IncludeStrings
            .Aggregate(query, (current, include) => current.Include(include));

        // Apply ordering
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        // Apply grouping
        if (specification.GroupBy != null)
        {
            query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
        }

        // Apply split query if specified (better performance for multiple includes)
        if (specification.IsSplitQuery)
        {
            query = query.AsSplitQuery();
        }

        // Apply paging (must be after ordering)
        if (specification.Skip.HasValue)
        {
            query = query.Skip(specification.Skip.Value);
        }

        if (specification.Take.HasValue)
        {
            query = query.Take(specification.Take.Value);
        }

        return query;
    }
}
