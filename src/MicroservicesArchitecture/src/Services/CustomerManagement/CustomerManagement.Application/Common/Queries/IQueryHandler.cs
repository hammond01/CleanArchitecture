namespace CustomerManagement.Application.Common.Queries;

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
  Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
