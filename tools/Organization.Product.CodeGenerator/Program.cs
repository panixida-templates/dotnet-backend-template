using System.Text.Json;
using System.Text.Json.Serialization;

using Organization.Product.CodeGenerator;

using Scriban.Syntax;

try
{
    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };
    var configurationPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
    await using var configuration = File.OpenRead(configurationPath);
    var settings = await JsonSerializer.DeserializeAsync<GeneratorSettings>(configuration, options);
    if (settings?.Jobs is not { Count: > 0 } jobs)
    {
        Console.WriteLine("Configure at least one entry in jobs in the generator's appsettings.json.");
        return 0;
    }

    var repository = RepositoryLayout.Find(AppContext.BaseDirectory);
    var files = await CrudGenerator.RunJobsAsync(repository, jobs);
    foreach (var file in files)
    {
        Console.WriteLine($"{file.Action}: {Path.GetRelativePath(repository.Root, file.Path)}");
    }

    Console.WriteLine($"Processed {files.Count} CRUD files.");
    if (jobs.Any(job => job.NeedMigrate))
    {
        Console.WriteLine("needMigrate is reserved for future use. Run the EF migrator separately.");
    }
    return 0;
}
catch (Exception exception) when (exception is IOException or UnauthorizedAccessException
    or InvalidOperationException or ArgumentException or JsonException or System.ComponentModel.Win32Exception
    or BadImageFormatException or TypeLoadException or ScriptRuntimeException)
{
    await Console.Error.WriteLineAsync(exception.Message);
    return 1;
}
