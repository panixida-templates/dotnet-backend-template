using Domain.Abstractions;

namespace Domain.Users.ValueObjects;

public sealed class BirthDate : ValueObject
{
    private BirthDate(DateOnly value)
    {
        Value = value;
    }

    public DateOnly Value { get; }

    public static BirthDate Create(DateOnly value)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (value > today)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "Birth date cannot be in the future.");
        }

        return new BirthDate(value);
    }

    public int GetAge(DateOnly onDate)
    {
        if (onDate < Value)
        {
            throw new ArgumentOutOfRangeException(
                nameof(onDate),
                "The calculation date cannot be earlier than birth date.");
        }

        var years = onDate.Year - Value.Year;

        var hadBirthdayThisYear = onDate.Month > Value.Month
            || (onDate.Month == Value.Month && onDate.Day >= Value.Day);

        if (!hadBirthdayThisYear)
        {
            years--;
        }

        return years;
    }

    public bool IsAtLeast(int age, DateOnly onDate)
    {
        return GetAge(onDate) >= age;
    }

    public void EnsureAtLeast(int age, DateOnly onDate)
    {
        if (!IsAtLeast(age, onDate))
        {
            throw new InvalidOperationException(
                $"User must be at least {age} years old.");
        }
    }

    public override string ToString()
    {
        return Value.ToString("yyyy-MM-dd");
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}