namespace Domain.Abstractions.ResultPattern;

public class Result
{
    protected static readonly Error[] EmptyErrors = [];

    protected Result(bool isSuccess, Error[] errors)
    {
        if (isSuccess && errors.Length > 0)
        {
            throw new ArgumentException("Successful result cannot contain errors.", nameof(errors));
        }

        if (!isSuccess && errors.Length == 0)
        {
            throw new ArgumentException("Failed result must contain at least one error.", nameof(errors));
        }

        IsSuccess = isSuccess;
        Errors = errors;
    }

    public bool IsSuccess { get; }

    public bool IsFailure
    {
        get
        {
            return !IsSuccess;
        }
    }

    public IReadOnlyList<Error> Errors { get; }

    public Error FirstError
    {
        get
        {
            if (IsSuccess)
            {
                throw new InvalidOperationException("Successful result does not contain errors.");
            }

            return Errors[0];
        }
    }

    public static Result Success()
    {
        return new Result(true, EmptyErrors);
    }

    public static Result<TValue> Success<TValue>(TValue value)
    {
        return new Result<TValue>(value);
    }

    public static Result Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new Result(false, [error]);
    }

    public static Result Failure(IEnumerable<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        var materializedErrors = errors.Where(item => item is not null).ToArray();

        if (materializedErrors.Length == 0)
        {
            throw new ArgumentException("Failed result must contain at least one error.", nameof(errors));
        }

        return new Result(false, materializedErrors);
    }

    public static Result<TValue> Failure<TValue>(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new Result<TValue>([error]);
    }

    public static Result<TValue> Failure<TValue>(IEnumerable<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        var materializedErrors = errors.Where(static item => item is not null).ToArray();

        if (materializedErrors.Length == 0)
        {
            throw new ArgumentException("Failed result must contain at least one error.", nameof(errors));
        }

        return new Result<TValue>(materializedErrors);
    }

    public static Result Combine(params Result[] results)
    {
        ArgumentNullException.ThrowIfNull(results);

        var errors = results
            .Where(item => item.IsFailure)
            .SelectMany(item => item.Errors)
            .ToArray();

        if (errors.Length == 0)
        {
            return Success();
        }

        return Failure(errors);
    }
}

public sealed class Result<TValue> : Result
{
    private readonly TValue? value;

    internal Result(TValue value)
        : base(true, EmptyErrors)
    {
        this.value = value;
    }

    internal Result(Error[] errors)
        : base(false, errors)
    {
        value = default;
    }

    public TValue Value
    {
        get
        {
            if (IsFailure)
            {
                throw new InvalidOperationException("Cannot access value of failed result.");
            }

            return value!;
        }
    }

    public TValue? ValueOrDefault
    {
        get
        {
            return IsSuccess ? value : default;
        }
    }

    public bool TryGetValue(out TValue? resultValue)
    {
        if (IsSuccess)
        {
            resultValue = value;
            return true;
        }

        resultValue = default;
        return false;
    }
}
