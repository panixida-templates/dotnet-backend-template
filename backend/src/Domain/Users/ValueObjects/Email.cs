using System.Net.Mail;

using Domain.Abstractions;

namespace Domain.Users.ValueObjects;

public sealed class Email : ValueObject
{
    private const int MaxLength = 320;

    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Email Create(string value)
    {
        var normalizedValue = value.Trim().ToLowerInvariant();

        if (normalizedValue.Length > MaxLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                $"Email cannot be longer than {MaxLength} characters.");
        }

        try
        {
            var mailAddress = new MailAddress(normalizedValue);

            if (!string.Equals(mailAddress.Address, normalizedValue, StringComparison.Ordinal))
            {
                throw new ArgumentException("Email has invalid format.", nameof(value));
            }
        }
        catch (FormatException)
        {
            throw new ArgumentException("Email has invalid format.", nameof(value));
        }

        return new Email(normalizedValue);
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
