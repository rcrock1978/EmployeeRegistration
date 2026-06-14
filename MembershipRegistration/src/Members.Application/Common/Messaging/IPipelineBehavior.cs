using Members.Application.Common.Results;

namespace Members.Application.Common.Messaging;

public interface IPipelineBehavior
{
    Task<Result<TResponse>> HandleAsync<TResponse>(
        object request,
        Func<Task<Result<TResponse>>> nextHandler,
        CancellationToken cancellationToken);
}
