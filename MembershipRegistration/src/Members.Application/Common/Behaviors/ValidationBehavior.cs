using FluentValidation;
using Members.Application.Common.Results;
using Members.Application.Common.Messaging;

namespace Members.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest> : IPipelineBehavior
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<Result<TResponse>> HandleAsync<TResponse>(
        object request,
        Func<Task<Result<TResponse>>> nextHandler,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await nextHandler();

        var context = new ValidationContext<TRequest>((TRequest)request);
        var failures = (await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
        ))
        .SelectMany(r => r.Errors)
        .Where(f => f is not null)
        .ToList();

        if (failures.Count == 0)
            return await nextHandler();

        var errors = failures
            .Select(f => new FieldError(f.PropertyName, f.ErrorCode, f.ErrorMessage))
            .ToList();

        return Result.Failure<TResponse>(new AppError(
            "Validation.Failed",
            "One or more fields are invalid.",
            errors));
    }
}
