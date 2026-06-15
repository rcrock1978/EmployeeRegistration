using FluentValidation;
using Members.Application.Common.Results;
using Members.Application.Common.Messaging;

namespace Members.Application.Common.Behaviors;

public sealed class ValidationBehavior : IPipelineBehavior
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationBehavior(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<Result<TResponse>> HandleAsync<TResponse>(
        object request,
        Func<Task<Result<TResponse>>> nextHandler,
        CancellationToken cancellationToken)
    {
        var requestType = request.GetType();
        var validatorType = typeof(IValidator<>).MakeGenericType(requestType);
        var validator = _serviceProvider.GetService(validatorType) as IValidator;

        if (validator is null)
            return await nextHandler();

        try
        {
            var contextType = typeof(ValidationContext<>).MakeGenericType(requestType);
            var context = (IValidationContext)Activator.CreateInstance(contextType, request)!;

            var result = await validator.ValidateAsync(context, cancellationToken);

            if (result.IsValid)
                return await nextHandler();

            var errors = result.Errors
                .Select(f => new FieldError(f.PropertyName, f.ErrorCode, f.ErrorMessage))
                .ToList();

            return Result.Failure<TResponse>(new AppError(
                "Validation.Failed",
                "One or more fields are invalid.",
                errors));
        }
        catch (NullReferenceException)
        {
            return Result.Failure<TResponse>(new AppError(
                "Validation.Failed",
                "The request body is missing required fields.",
                new List<FieldError>
                {
                    new("request", "Required", "The request body must contain all required fields.")
                }));
        }
    }
}
