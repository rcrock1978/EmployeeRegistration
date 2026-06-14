using Members.Application.Common.Results;

namespace Members.Application.Common.Messaging;

public interface ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken);
}

public interface ICommand<TResponse>
{
}
