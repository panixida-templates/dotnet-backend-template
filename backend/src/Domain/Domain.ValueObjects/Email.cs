using System.Net.Mail;

using Domain.Exceptions;
using Domain.ValueObjects.Core;

namespace Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException("Email cannot be empty.");
        }

        var normalized = value.Trim().ToLowerInvariant();

        try
        {
            _ = new MailAddress(normalized);
        }
        catch
        {
            throw new DomainValidationException("Email has invalid format.");
        }

        return new Email(normalized);
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
