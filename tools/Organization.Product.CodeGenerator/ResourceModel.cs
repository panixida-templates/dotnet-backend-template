using System.Reflection;
using System.Text.RegularExpressions;

namespace Organization.Product.CodeGenerator;

internal sealed record ResourceModel(
    string Module,
    string ModuleName,
    string Feature,
    string Resource,
    string Domain,
    string Id,
    string Route,
    IReadOnlyList<CrudField> Fields,
    IReadOnlyList<CrudField> CreateFields,
    IReadOnlyList<CrudField> UpdateFields,
    IReadOnlyList<CrudField> DetailsFields,
    IReadOnlyList<CrudField> ListFields)
{
    private const string AggregateRootType = "PANiXiDA.Core.Domain.AggregateRoots.AggregateRoot`1";
    private const string ResultType = "PANiXiDA.Core.ResultPattern.Result";
    private const string GenericResultType = ResultType + "`1";

    private static readonly Dictionary<Type, string> s_scalarNames = new()
    {
        [typeof(bool)] = "bool",
        [typeof(byte)] = "byte",
        [typeof(short)] = "short",
        [typeof(int)] = "int",
        [typeof(long)] = "long",
        [typeof(float)] = "float",
        [typeof(double)] = "double",
        [typeof(decimal)] = "decimal",
        [typeof(string)] = "string",
        [typeof(Guid)] = nameof(Guid),
        [typeof(DateOnly)] = nameof(DateOnly),
        [typeof(TimeOnly)] = nameof(TimeOnly),
        [typeof(DateTime)] = nameof(DateTime),
        [typeof(DateTimeOffset)] = nameof(DateTimeOffset)
    };

    public CrudField[] CreateValidationFields => [.. CreateFields.Where(item => item.HasValidation)];

    public CrudField[] UpdateValidationFields => [.. UpdateFields.Where(item => item.HasValidation)];

    public bool HasCompoundFields => Fields.Any(item => item.Composite);

    public bool HasCompoundDetails => DetailsFields.Any(item => item.Composite);

    public bool HasCompoundList => ListFields.Any(item => item.Composite);

    internal static ResourceModel Create(Assembly domainAssembly, CrudTarget target, ResourceSettings settings)
    {
        if (!IsRoute(settings.Route))
        {
            throw new InvalidOperationException(
                $"'{target.Aggregate}' requires a lowercase, single-segment route.");
        }

        var aggregate = domainAssembly.GetType(target.Aggregate);
        if (aggregate is not { IsPublic: true, IsClass: true, IsAbstract: false, IsGenericType: false })
        {
            throw new InvalidOperationException(
                $"'{target.Aggregate}' must be a public concrete aggregate in '{target.Project.ProjectFile}'.");
        }

        var id = GetId(aggregate, target.Domain);
        var nullability = new NullabilityInfoContext();
        var fields = aggregate.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(property => property.Name != "Id"
                && property.GetMethod is { IsPublic: true }
                && property.GetIndexParameters().Length == 0
                && property.SetMethod is not null)
            .Select(property => DescribeField(property, nullability))
            .ToArray();

        var create = GetOperation(aggregate, "Create", isStatic: true, payload: aggregate);
        var update = GetOperation(aggregate, "Update", isStatic: false, payload: null);
        var readableFields = fields.Prepend(new CrudField("Id", nameof(Guid), false, null, null, false)).ToArray();

        return new ResourceModel(
            target.Project.Module,
            target.Project.ModuleName,
            target.Feature,
            target.Resource,
            target.Domain,
            id.Name,
            settings.Route,
            fields,
            BindParameters(create, fields, nullability),
            BindParameters(update, fields, nullability),
            SelectProjection(settings.DetailsFields, readableFields, nameof(settings.DetailsFields)),
            SelectProjection(settings.ListFields, readableFields, nameof(settings.ListFields)));
    }

    private static Type GetId(Type aggregate, string domain)
    {
        var baseType = aggregate.BaseType;
        if (baseType is null
            || !baseType.IsGenericType
            || baseType.GetGenericTypeDefinition().FullName != AggregateRootType)
        {
            throw new InvalidOperationException($"'{aggregate.FullName}' must derive from AggregateRoot<TId>.");
        }

        var id = baseType.GetGenericArguments()[0];
        if (id.Namespace != domain
            || id.GetProperty("Value")?.PropertyType != typeof(Guid)
            || !HasFactory(id, typeof(Guid), allowNullableResult: false))
        {
            throw new InvalidOperationException(
                $"'{aggregate.FullName}' requires a typed Guid ID in the same namespace with Value and Create(Guid).");
        }

        return id;
    }

    private static MethodInfo GetOperation(Type aggregate, string name, bool isStatic, Type? payload)
    {
        var flags = BindingFlags.Public | BindingFlags.DeclaredOnly;
        flags |= isStatic ? BindingFlags.Static : BindingFlags.Instance;
        var methods = aggregate.GetMethods(flags)
            .Where(method => method.Name == name
                && !method.IsGenericMethod
                && MatchesResult(method.ReturnType, payload))
            .ToArray();
        return methods.Length == 1
            ? methods[0]
            : throw new InvalidOperationException(
                $"'{aggregate.FullName}' requires exactly one public '{name}' method with the expected Result type.");
    }

    private static CrudField DescribeField(PropertyInfo property, NullabilityInfoContext nullability)
    {
        var declaredType = property.PropertyType;
        var type = Nullable.GetUnderlyingType(declaredType) ?? declaredType;
        var optional = IsOptional(declaredType, nullability.Create(property));
        if (s_scalarNames.TryGetValue(type, out var scalar))
        {
            return new CrudField(property.Name, optional ? scalar + "?" : scalar, optional, null, null, false);
        }

        var value = type.GetProperty("Value") ?? type.GetProperty("Name");
        if (type.IsPublic
            && !type.IsGenericType
            && value?.GetMethod is { IsPublic: true, IsStatic: false }
            && s_scalarNames.TryGetValue(value.PropertyType, out scalar))
        {
            var factoryInput = optional && value.PropertyType.IsValueType
                ? typeof(Nullable<>).MakeGenericType(value.PropertyType)
                : value.PropertyType;
            if (HasFactory(type, factoryInput, allowNullableResult: optional))
            {
                var maxLength = type.GetField("MaxLength", BindingFlags.Public | BindingFlags.Static);
                var valueMember = optional && type.IsValueType ? $"Value.{value.Name}" : value.Name;
                return new CrudField(
                    property.Name,
                    optional ? scalar + "?" : scalar,
                    optional,
                    $"global::{type.FullName}",
                    valueMember,
                    maxLength is { IsLiteral: true } && maxLength.FieldType == typeof(int));
            }
        }

        var propertyName = $"{property.DeclaringType?.FullName}.{property.Name}";
        var components = GetComponents(type, nullability, propertyName);
        if (components is not null)
        {
            return new CrudField(
                property.Name,
                string.Empty,
                optional,
                $"global::{type.FullName}",
                null,
                false,
                components,
                optional && type.IsValueType);
        }

        throw new InvalidOperationException(
            $"'{propertyName}' requires a scalar or a value object with a matching Create factory.");
    }

    private static ValueComponent[]? GetComponents(Type type, NullabilityInfoContext nullability, string propertyName)
    {
        if (!type.IsPublic || type.IsGenericType)
        {
            return null;
        }

        var candidates = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(method => method.Name == "Create"
                && !method.IsGenericMethod
                && MatchesResult(method.ReturnType, type))
            .Select(method => method.GetParameters())
            .Where(parameters => parameters.Length > 1)
            .ToArray();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.GetMethod is { IsPublic: true } && property.GetIndexParameters().Length == 0)
            .ToArray();
        var matches = candidates
            .Select(parameters => BindComponents(parameters, properties, nullability))
            .OfType<ValueComponent[]>()
            .ToArray();
        if (matches.Length > 1)
        {
            throw new InvalidOperationException(
                $"'{propertyName}' has more than one matching compound Create factory.");
        }

        return matches.FirstOrDefault();
    }

    private static ValueComponent[]? BindComponents(
        IReadOnlyList<ParameterInfo> parameters,
        IReadOnlyList<PropertyInfo> properties,
        NullabilityInfoContext nullability)
    {
        var components = new List<ValueComponent>(parameters.Count);
        foreach (var parameter in parameters)
        {
            var property = properties.SingleOrDefault(candidate =>
                string.Equals(candidate.Name, parameter.Name, StringComparison.OrdinalIgnoreCase));
            var optional = IsOptional(parameter.ParameterType, nullability.Create(parameter));
            var underlying = Nullable.GetUnderlyingType(parameter.ParameterType) ?? parameter.ParameterType;
            if (property is null
                || property.PropertyType != parameter.ParameterType
                || optional != IsOptional(property.PropertyType, nullability.Create(property))
                || !s_scalarNames.TryGetValue(underlying, out var scalar)
                || components.Any(component => component.Name == property.Name))
            {
                return null;
            }

            components.Add(new ValueComponent(property.Name, optional ? scalar + "?" : scalar));
        }

        return [.. components];
    }

    private static bool HasFactory(Type type, Type argumentType, bool allowNullableResult)
    {
        var factory = type.GetMethod("Create", BindingFlags.Public | BindingFlags.Static, [argumentType]);
        var resultType = factory?.ReturnType;
        if (resultType is null || !IsGenericResult(resultType))
        {
            return false;
        }

        var payload = resultType.GetGenericArguments()[0];
        return payload == type || (allowNullableResult && Nullable.GetUnderlyingType(payload) == type);
    }

    private static bool MatchesResult(Type type, Type? payload) =>
        payload is null
            ? type.FullName == ResultType
            : IsGenericResult(type) && type.GetGenericArguments()[0] == payload;

    private static bool IsGenericResult(Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition().FullName == GenericResultType;

    private static CrudField[] BindParameters(
        MethodInfo method, IReadOnlyList<CrudField> fields, NullabilityInfoContext nullability)
    {
        var parameters = method.GetParameters();
        var bound = new List<CrudField>(parameters.Length);
        foreach (var parameter in parameters)
        {
            var field = fields.SingleOrDefault(
                item => string.Equals(item.Name, parameter.Name, StringComparison.OrdinalIgnoreCase));
            var underlying = Nullable.GetUnderlyingType(parameter.ParameterType) ?? parameter.ParameterType;
            var optional = IsOptional(parameter.ParameterType, nullability.Create(parameter));
            if (field is null
                || !MatchesParameter(field, underlying, optional)
                || bound.Contains(field))
            {
                var parameterName = $"{method.DeclaringType?.Name}.{method.Name}({parameter.Name})";
                throw new InvalidOperationException(
                    $"'{parameterName}' must match a writable aggregate property and its input type.");
            }

            bound.Add(field);
        }

        return [.. bound];
    }

    private static bool MatchesParameter(CrudField field, Type parameterType, bool optional)
    {
        if (field.Composite)
        {
            return field.Optional == optional && field.ValueObject == $"global::{parameterType.FullName}";
        }

        return s_scalarNames.TryGetValue(parameterType, out var scalar)
            && field.Type == (optional ? scalar + "?" : scalar);
    }

    private static CrudField[] SelectProjection(
        IReadOnlyList<string>? names, IReadOnlyList<CrudField> fields, string setting)
    {
        if (names is null || !names.Contains("Id") || names.Distinct(StringComparer.Ordinal).Count() != names.Count)
        {
            throw new InvalidOperationException($"'{setting}' must contain Id and distinct property names.");
        }

        return [.. names.Select(name => fields.SingleOrDefault(field => field.Name == name)
            ?? throw new InvalidOperationException($"Unknown property '{name}' in '{setting}'."))];
    }

    private static bool IsOptional(Type type, NullabilityInfo nullability) =>
        Nullable.GetUnderlyingType(type) is not null || nullability.ReadState == NullabilityState.Nullable;

    private static bool IsRoute(string? route) =>
        Regex.IsMatch(
            route ?? string.Empty,
            @"^[a-z][a-z0-9]*(?:-[a-z0-9]+)*\z",
            RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);
}

internal sealed record CrudField(
    string Name,
    string Type,
    bool Optional,
    string? ValueObject,
    string? ValueMember,
    bool HasMaxLength,
    IReadOnlyList<ValueComponent>? Components = null,
    bool NullableStruct = false)
{
    public bool Composite => Components is not null;

    public string LocalName => char.ToLowerInvariant(Name[0]) + Name[1..];

    public bool RequiredString => Type == "string";

    public bool HasValidation => Composite ? !Optional : RequiredString || ValueObject is not null;
}

internal sealed record ValueComponent(string Name, string Type);
