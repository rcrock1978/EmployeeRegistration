namespace Members.Application.Common.Results;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public AppError? Error { get; }

    protected Result(bool isSuccess, AppError? error)
    {
        if (isSuccess && error is not null)
            throw new InvalidOperationException("Success result cannot have an error.");
        if (!isSuccess && error is null)
            throw new InvalidOperationException("Failure result must have an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);

    public static Result<TValue> Success<TValue>(TValue value) =>
        new(value, true, null);

    public static Result Failure(AppError error) => new(false, error);

    public static Result<TValue> Failure<TValue>(AppError error) =>
        new(default, false, error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result(TValue? value, bool isSuccess, AppError? error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of a failed result.");
}

public record AppError(string Code, string Message, List<FieldError>? Details = null);

public record FieldError(string Field, string Code, string Message);
