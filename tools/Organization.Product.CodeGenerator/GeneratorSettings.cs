namespace Organization.Product.CodeGenerator;

internal sealed record GeneratorSettings(IReadOnlyList<JobSettings>? Jobs);

internal sealed record JobSettings(
    string PathToModels,
    IReadOnlyList<ResourceSettings> Models,
    IReadOnlyList<string> GeneratedFiles,
    IReadOnlyDictionary<string, string>? Templates,
    IReadOnlyDictionary<string, string>? OutputDirectory,
    bool OverwriteExisting,
    bool DeleteGenerated,
    bool NeedMigrate);

internal sealed record ResourceSettings(
    string Aggregate,
    string Route,
    IReadOnlyList<string> DetailsFields,
    IReadOnlyList<string> ListFields);
