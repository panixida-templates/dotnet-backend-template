using Domain.Abstractions;
using Domain.Abstractions.ResultPattern;

namespace Domain.Users.ValueObjects;

public sealed class UserName : ValueObject
{
    private const int MaxLength = 200;

    private UserName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<UserName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<UserName>(
                Error.Validation("User name cannot be empty.")
                .WithField(nameof(PhoneNumber)));
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > MaxLength)
        {
            return Result.Failure<UserName>(
                Error.Validation($"User name cannot be longer than {MaxLength} characters.")
                .WithField(nameof(PhoneNumber)));
        }

        return Result.Success(new UserName(normalizedValue));
    }

    public override string ToString()
    {
        return Value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
