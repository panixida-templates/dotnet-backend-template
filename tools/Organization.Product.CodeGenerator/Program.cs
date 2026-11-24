using System.Text.Json;
using System.Text.Json.Serialization;

using Organization.Product.CodeGenerator;
using Organization.Product.Module.Domain;

try
{
    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };
    var configurationPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
    using var configuration = File.OpenRead(configurationPath);
    var settings = JsonSerializer.Deserialize<GeneratorSettings>(configuration, options);
    if (settings?.Crud?.Resources is not { Count: > 0 } resources)
    {
        Console.WriteLine("Configure at least one resource in Crud:Resources in the generator's appsettings.json.");
        return 0;
    }

    var repository = RepositoryLayout.Find(AppContext.BaseDirectory, DomainAssembly.Instance);
    var files = CrudGenerator.Generate(repository, resources);
    foreach (var file in files)
    {
        Console.WriteLine(Path.GetRelativePath(repository.Root, file));
    }

    Console.WriteLine($"Created {files.Count} CRUD files.");
    return 0;
}
catch (Exception exception) when (exception is IOException or UnauthorizedAccessException
    or InvalidOperationException or JsonException)
{
    Console.Error.WriteLine(exception.Message);
    return 1;
}
