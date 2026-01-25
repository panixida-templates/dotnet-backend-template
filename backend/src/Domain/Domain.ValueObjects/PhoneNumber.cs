using Domain.Exceptions;
using Domain.ValueObjects.Core;

namespace Domain.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException("Phone number cannot be empty.");
        }

        var trimmed = value.Trim();

        var digits = new string([.. trimmed.Where(char.IsDigit)]);

        if (digits.Length < 7 || digits.Length > 15)
        {
            throw new DomainValidationException("Phone number has invalid length.");
        }

        var normalized = $"+{digits}";

        return new PhoneNumber(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
