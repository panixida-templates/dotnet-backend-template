namespace Domain.Abstractions.ResultPattern;

public sealed record Error
{
    public const string FieldMetadataKey = "field";

    public Error(
        string message,
        ErrorType type,
        IReadOnlyDictionary<string, object?>? metadata = null)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Error message cannot be empty.", nameof(message));
        }

        Message = message;
        Type = type;
        Metadata = metadata ?? new Dictionary<string, object?>();
    }

    public string Message { get; }
    public ErrorType Type { get; }
    public IReadOnlyDictionary<string, object?> Metadata { get; }

    public static Error Validation(string message)
    {
        return new Error(message, ErrorType.Validation);
    }

    public static Error NotFound(string message)
    {
        return new Error(message, ErrorType.NotFound);
    }

    public static Error Conflict(string message)
    {
        return new Error(message, ErrorType.Conflict);
    }

    public static Error Unauthorized(string message)
    {
        return new Error(message, ErrorType.Unauthorized);
    }

    public static Error Forbidden(string message)
    {
        return new Error(message, ErrorType.Forbidden);
    }

    public static Error Failure(string message)
    {
        return new Error(message, ErrorType.Failure);
    }

    public static Error Unexpected(string message)
    {
        return new Error(message, ErrorType.Unexpected);
    }

    public Error WithMetadata(string key, object? value)
    {
        var metadata = new Dictionary<string, object?>(Metadata)
        {
            [key] = value
        };

        return new Error(Message, Type, metadata);
    }

    public Error WithField(string field)
    {
        if (string.IsNullOrWhiteSpace(field))
        {
            throw new ArgumentException("Field cannot be empty.", nameof(field));
        }

        return WithMetadata(FieldMetadataKey, field);
    }
}
