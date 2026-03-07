using Domain.Abstractions;

namespace Domain.Users.ValueObjects;

public sealed class Avatar : ValueObject
{
    private const int MaxLength = 2048;

    private Avatar(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Avatar? Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        value = value.Trim();

        if (value.Length > MaxLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                $"Avatar cannot be longer than {MaxLength} characters.");
        }

        return new Avatar(value);
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
