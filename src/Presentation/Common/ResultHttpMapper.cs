using Microsoft.AspNetCore.Http;

namespace Presentation.Http.Common;

internal static class ResultHttpMapper
{
    public static IResult ToHttpResult(
        this Result result,
        Func<IResult> onSuccess)
    {
        if (result.IsSuccess)
        {
            return onSuccess();
        }

        return CreateProblem(result.Errors);
    }

    public static IResult ToHttpResult<TValue>(
        this Result<TValue> result,
        Func<TValue, IResult> onSuccess)
    {
        if (result.IsSuccess)
        {
            return onSuccess(result.Value);
        }

        return CreateProblem(result.Errors);
    }

    private static IResult CreateProblem(IReadOnlyList<Error> errors)
    {
        var firstError = errors[0];
        var statusCode = GetStatusCode(firstError.Type);

        if (firstError.Type == ErrorType.Validation)
        {
            var validationErrors = errors
                .GroupBy(GetFieldName)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(item => item.Message)
                        .Distinct()
                        .ToArray());

            return TypedResults.ValidationProblem(
                errors: validationErrors,
                title: "One or more validation errors occurred.");
        }

        return TypedResults.Problem(
            statusCode: statusCode,
            title: GetTitle(firstError.Type),
            detail: firstError.Message);
    }

    private static int GetStatusCode(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Failure => StatusCodes.Status400BadRequest,
            ErrorType.Unexpected => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private static string GetTitle(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => "Validation failed",
            ErrorType.NotFound => "Resource not found",
            ErrorType.Conflict => "Conflict",
            ErrorType.Unauthorized => "Unauthorized",
            ErrorType.Forbidden => "Forbidden",
            ErrorType.Failure => "Request failed",
            ErrorType.Unexpected => "Server error",
            _ => "Server error"
        };
    }

    private static string GetFieldName(Error error)
    {
        if (error.Metadata.TryGetValue(Error.FieldMetadataKey, out var field) &&
            field is string fieldName &&
            !string.IsNullOrWhiteSpace(fieldName))
        {
            return fieldName;
        }

        return "general";
    }
}
