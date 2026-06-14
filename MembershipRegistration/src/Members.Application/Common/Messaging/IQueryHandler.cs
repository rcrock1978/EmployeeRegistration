using Members.Application.Common.Results;

namespace Members.Application.Common.Messaging;

public interface IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}

public interface IQuery<TResponse>
{
}
