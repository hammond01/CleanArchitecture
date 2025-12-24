using System.Linq.Expressions;

namespace ProductManager.Domain.Common;

/// <summary>
/// Specification pattern interface for building complex queries with includes
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Filter criteria (WHERE clause)
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// Navigation properties to include (eager loading)
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// String-based includes for nested properties (e.g., "Category.Parent")
    /// </summary>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// Order by expression
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// Order by descending expression
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// Group by expression
    /// </summary>
    Expression<Func<T, object>>? GroupBy { get; }

    /// <summary>
    /// Number of records to take (for pagination)
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// Number of records to skip (for pagination)
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// Enable query splitting for complex queries
    /// </summary>
    bool IsSplitQuery { get; }

    /// <summary>
    /// Disable tracking for read-only queries (better performance)
    /// </summary>
    bool IsNoTracking { get; }
}
