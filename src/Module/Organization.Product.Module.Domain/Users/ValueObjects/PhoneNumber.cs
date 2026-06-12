using System.Text;

namespace Organization.Product.Module.Domain.Users.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    private PhoneNumber(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<PhoneNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<PhoneNumber>(
                Error.Validation("Phone number cannot be empty.")
                .WithField(nameof(PhoneNumber)));
        }

        var normalizeResult = Normalize(value);

        if (normalizeResult.IsFailure)
        {
            return Result.Failure<PhoneNumber>(normalizeResult.Errors);
        }

        var normalizedValue = normalizeResult.Value;
        var digitsCount = CountDigits(normalizedValue);

        if (digitsCount < 8 || digitsCount > 15)
        {
            return Result.Failure<PhoneNumber>(
                Error.Validation("Phone number must contain from 8 to 15 digits.")
                .WithField(nameof(PhoneNumber)));
        }

        return Result.Success(new PhoneNumber(normalizedValue));
    }

    public override string ToString()
    {
        return Value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    private static Result<string> Normalize(string value)
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
            return Result.Failure<string>(
                Error.Validation("Phone number has invalid format.")
                .WithField(nameof(PhoneNumber)));
        }

        return Result.Success(builder.ToString());
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
