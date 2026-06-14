using Members.Application.Common.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Members.Application.Common.Messaging;

public sealed class Sender : ISender
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<IPipelineBehavior> _pipelineBehaviors;

    public Sender(IServiceProvider serviceProvider, IEnumerable<IPipelineBehavior> pipelineBehaviors)
    {
        _serviceProvider = serviceProvider;
        _pipelineBehaviors = pipelineBehaviors;
    }

    public async Task<Result<TResponse>> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<ICommand<TResponse>, TResponse>>();

        Func<Task<Result<TResponse>>> execute = () => handler.Handle((dynamic)command, cancellationToken);

        foreach (var behavior in _pipelineBehaviors.Reverse())
        {
            var current = execute;
            execute = () => behavior.HandleAsync((dynamic)command, current, cancellationToken);
        }

        return await execute();
    }

    public async Task<Result<TResponse>> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        var handler = _serviceProvider.GetRequiredService<IQueryHandler<IQuery<TResponse>, TResponse>>();

        Func<Task<Result<TResponse>>> execute = () => handler.Handle((dynamic)query, cancellationToken);

        foreach (var behavior in _pipelineBehaviors.Reverse())
        {
            var current = execute;
            execute = () => behavior.HandleAsync((dynamic)query, current, cancellationToken);
        }

        return await execute();
    }
}
