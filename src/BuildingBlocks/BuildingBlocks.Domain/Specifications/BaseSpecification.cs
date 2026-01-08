using System.Linq.Expressions;

namespace BuildingBlocks.Domain.Specifications;

/// <summary>
/// Base specification class with fluent API for building queries
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public abstract class BaseSpecification<T> : ISpecification<T>
{
    protected BaseSpecification()
    {
    }

    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    public Expression<Func<T, bool>>? Criteria { get; private set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }
    public Expression<Func<T, object>>? GroupBy { get; private set; }
    public int? Take { get; private set; }
    public int? Skip { get; private set; }
    public bool IsSplitQuery { get; private set; }
    public bool IsNoTracking { get; private set; } = true; // Default to no tracking for better performance

    /// <summary>
    /// Add navigation property to include (eager loading)
    /// </summary>
    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    /// <summary>
    /// Add string-based include for nested properties
    /// Example: "Category.Parent"
    /// </summary>
    protected virtual void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    /// <summary>
    /// Apply ordering (ascending)
    /// </summary>
    protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    /// <summary>
    /// Apply ordering (descending)
    /// </summary>
    protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
        OrderByDescending = orderByDescExpression;
    }

    /// <summary>
    /// Apply grouping
    /// </summary>
    protected virtual void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
    {
        GroupBy = groupByExpression;
    }

    /// <summary>
    /// Apply pagination
    /// </summary>
    protected virtual void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }

    /// <summary>
    /// Enable query splitting for better performance with multiple includes
    /// </summary>
    protected virtual void EnableSplitQuery()
    {
        IsSplitQuery = true;
    }

    /// <summary>
    /// Enable change tracking (for update scenarios)
    /// </summary>
    protected virtual void EnableTracking()
    {
        IsNoTracking = false;
    }

    /// <summary>
    /// Disable change tracking (for read-only scenarios - better performance)
    /// </summary>
    protected virtual void DisableTracking()
    {
        IsNoTracking = true;
    }
}
