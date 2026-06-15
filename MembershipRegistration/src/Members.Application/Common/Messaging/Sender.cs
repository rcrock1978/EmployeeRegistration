using System.Reflection;
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
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var handleMethod = handlerType.GetMethod("Handle", [command.GetType(), typeof(CancellationToken)])!;

        Func<Task<Result<TResponse>>> execute = () =>
            (Task<Result<TResponse>>)handleMethod.Invoke(handler, [command, cancellationToken])!;

        foreach (var behavior in _pipelineBehaviors.Reverse())
        {
            var current = execute;
            execute = () => behavior.HandleAsync((dynamic)command, current, cancellationToken);
        }

        return await execute();
    }

    public async Task<Result<TResponse>> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var handleMethod = handlerType.GetMethod("Handle", [query.GetType(), typeof(CancellationToken)])!;

        Func<Task<Result<TResponse>>> execute = () =>
            (Task<Result<TResponse>>)handleMethod.Invoke(handler, [query, cancellationToken])!;

        foreach (var behavior in _pipelineBehaviors.Reverse())
        {
            var current = execute;
            execute = () => behavior.HandleAsync((dynamic)query, current, cancellationToken);
        }

        return await execute();
    }
}
