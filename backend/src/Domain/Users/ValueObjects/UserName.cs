using Domain.Abstractions;

namespace Domain.Users.ValueObjects;

public sealed class UserName : ValueObject
{
    private const int MaxLength = 200;

    private UserName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static UserName Create(string value)
    {
        value = value.Trim();

        if (value.Length > MaxLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                $"User name cannot be longer than {MaxLength} characters.");
        }

        return new UserName(value);
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
