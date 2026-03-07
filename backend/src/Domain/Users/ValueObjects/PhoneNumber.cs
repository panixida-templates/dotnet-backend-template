using System.Text;

using Domain.Abstractions;

namespace Domain.Users.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    private PhoneNumber(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static PhoneNumber Create(string value)
    {
        var normalizedValue = Normalize(value);
        var digitsCount = CountDigits(normalizedValue);

        if (digitsCount < 8 || digitsCount > 15)
        {
            throw new ArgumentException(
                "Phone number must contain from 8 to 15 digits.",
                nameof(value));
        }

        return new PhoneNumber(normalizedValue);
    }

    public override string ToString()
    {
        return Value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    private static string Normalize(string value)
    {
        var trimmedValue = value.Trim();
        var builder = new StringBuilder(trimmedValue.Length);

        for (var i = 0; i < trimmedValue.Length; i++)
        {
            var currentChar = trimmedValue[i];

            if (char.IsDigit(currentChar))
            {
                builder.Append(currentChar);
                continue;
            }

            if (currentChar == '+' && builder.Length == 0)
            {
                builder.Append(currentChar);
            }
        }

        if (builder.Length == 0)
        {
            throw new ArgumentException("Phone number has invalid format.", nameof(value));
        }

        return builder.ToString();
    }

    private static int CountDigits(string value)
    {
        var count = 0;

        for (var i = 0; i < value.Length; i++)
        {
            if (char.IsDigit(value[i]))
            {
                count++;
            }
        }

        return count;
    }
}
