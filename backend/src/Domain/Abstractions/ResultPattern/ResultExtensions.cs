namespace Domain.Abstractions.ResultPattern;

public static class ResultExtensions
{
    public static Result<TOut> Map<TOut>(this Result result, Func<TOut> map)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(map);

        if (result.IsFailure)
        {
            return Result.Failure<TOut>(result.Errors);
        }

        return Result.Success(map());
    }

    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> map)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(map);

        if (result.IsFailure)
        {
            return Result.Failure<TOut>(result.Errors);
        }

        return Result.Success(map(result.Value));
    }

    public static Result Bind<TIn>(this Result<TIn> result, Func<TIn, Result> bind)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(bind);

        if (result.IsFailure)
        {
            return Result.Failure(result.Errors);
        }

        return bind(result.Value);
    }

    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> bind)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(bind);

        if (result.IsFailure)
        {
            return Result.Failure<TOut>(result.Errors);
        }

        return bind(result.Value);
    }

    public static Result<TOut> Bind<TOut>(this Result result, Func<Result<TOut>> bind)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(bind);

        if (result.IsFailure)
        {
            return Result.Failure<TOut>(result.Errors);
        }

        return bind();
    }

    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(this Result<TIn> result, Func<TIn, Task<Result<TOut>>> bind)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(bind);

        if (result.IsFailure)
        {
            return Result.Failure<TOut>(result.Errors);
        }

        return await bind(result.Value);
    }

    public static async Task<Result<TOut>> BindAsync<TOut>(this Result result, Func<Task<Result<TOut>>> bind)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(bind);

        if (result.IsFailure)
        {
            return Result.Failure<TOut>(result.Errors);
        }

        return await bind();
    }

    public static Result<TIn> Ensure<TIn>(this Result<TIn> result, Func<TIn, bool> predicate, Error error)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(error);

        if (result.IsFailure)
        {
            return result;
        }

        return predicate(result.Value)
            ? result
            : Result.Failure<TIn>(error);
    }

    public static Result<TIn> Tap<TIn>(this Result<TIn> result, Action<TIn> action)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(action);

        if (result.IsFailure)
        {
            return result;
        }

        action(result.Value);
        return result;
    }

    public static TOut Match<TOut>(this Result result, Func<TOut> onSuccess, Func<IReadOnlyList<Error>, TOut> onFailure)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        return result.IsSuccess
            ? onSuccess()
            : onFailure(result.Errors);
    }

    public static TOut Match<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> onSuccess, Func<IReadOnlyList<Error>, TOut> onFailure)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        return result.IsSuccess
            ? onSuccess(result.Value)
            : onFailure(result.Errors);
    }
}
