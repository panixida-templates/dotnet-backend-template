using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Reflection;

namespace Domain.Abstractions;

public abstract class Enumeration<TEnumeration>(int id, string name) : IEquatable<TEnumeration>, IComparable<TEnumeration>
    where TEnumeration : Enumeration<TEnumeration>
{
    private static readonly Lazy<EnumerationCache> Cache = new(CreateCache);

    public int Id { get; } = id;
    public string Name { get; } = name;

    public static bool operator ==(Enumeration<TEnumeration>? left, Enumeration<TEnumeration>? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Id == right.Id;
    }

    public static bool operator !=(Enumeration<TEnumeration>? left, Enumeration<TEnumeration>? right)
    {
        return !(left == right);
    }

    public static bool operator <(Enumeration<TEnumeration>? left, Enumeration<TEnumeration>? right)
    {
        return Compare(left, right) < 0;
    }

    public static bool operator <=(Enumeration<TEnumeration>? left, Enumeration<TEnumeration>? right)
    {
        return Compare(left, right) <= 0;
    }

    public static bool operator >(Enumeration<TEnumeration>? left, Enumeration<TEnumeration>? right)
    {
        return Compare(left, right) > 0;
    }

    public static bool operator >=(Enumeration<TEnumeration>? left, Enumeration<TEnumeration>? right)
    {
        return Compare(left, right) >= 0;
    }

    public override string ToString()
    {
        return Name;
    }

    public virtual bool Equals(TEnumeration? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        return obj is TEnumeration other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(typeof(TEnumeration), Id);
    }

    public int CompareTo(TEnumeration? other)
    {
        if (other is null)
        {
            return 1;
        }

        return Id.CompareTo(other.Id);
    }

    public static IReadOnlyList<TEnumeration> GetAll()
    {
        return Cache.Value.Items;
    }

    public static TEnumeration FromId(int id)
    {
        if (Cache.Value.ById.TryGetValue(id, out var item))
        {
            return item;
        }

        throw new InvalidOperationException(
            $"'{id}' is not a valid id in {typeof(TEnumeration).Name}");
    }

    public static TEnumeration FromName(string name)
    {
        if (Cache.Value.ByName.TryGetValue(name, out var item))
        {
            return item;
        }

        throw new InvalidOperationException(
            $"'{name}' is not a valid name in {typeof(TEnumeration).Name}");
    }

    private static int Compare(Enumeration<TEnumeration>? left, Enumeration<TEnumeration>? right)
    {
        if (ReferenceEquals(left, right))
        {
            return 0;
        }

        if (left is null)
        {
            return -1;
        }

        if (right is null)
        {
            return 1;
        }

        return left.Id.CompareTo(right.Id);
    }

    private static EnumerationCache CreateCache()
    {
        var fields = typeof(TEnumeration)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        var itemsBuilder = ImmutableArray.CreateBuilder<TEnumeration>(fields.Length);
        var byId = new Dictionary<int, TEnumeration>(fields.Length);
        var byName = new Dictionary<string, TEnumeration>(fields.Length, StringComparer.Ordinal);

        foreach (var field in fields)
        {
            if (field.GetValue(null) is not TEnumeration item)
            {
                continue;
            }

            if (!byId.TryAdd(item.Id, item))
            {
                throw new InvalidOperationException(
                    $"Duplicate id '{item.Id}' in {typeof(TEnumeration).Name}");
            }

            if (!byName.TryAdd(item.Name, item))
            {
                throw new InvalidOperationException(
                    $"Duplicate name '{item.Name}' in {typeof(TEnumeration).Name}");
            }

            itemsBuilder.Add(item);
        }

        return new EnumerationCache(
            itemsBuilder.MoveToImmutable(),
            byId.ToFrozenDictionary(),
            byName.ToFrozenDictionary(StringComparer.Ordinal));
    }

    private sealed record EnumerationCache(
        ImmutableArray<TEnumeration> Items,
        FrozenDictionary<int, TEnumeration> ById,
        FrozenDictionary<string, TEnumeration> ByName);
}