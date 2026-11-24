namespace Organization.Product.CodeGenerator;

internal sealed record GeneratorSettings(CrudSettings? Crud);

internal sealed record CrudSettings(IReadOnlyList<ResourceSettings> Resources);

internal sealed record ResourceSettings(
    string Aggregate,
    string Route,
    IReadOnlyList<string> DetailsFields,
    IReadOnlyList<string> ListFields);
