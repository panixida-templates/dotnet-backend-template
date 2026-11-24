using Scriban;
using Scriban.Runtime;

namespace Organization.Product.CodeGenerator;

internal static class CrudGenerator
{
    internal static IReadOnlyList<string> Generate(
        RepositoryLayout repository,
        IReadOnlyList<ResourceSettings> settings)
    {
        var templates = LoadTemplates();
        var resources = settings.Select(setting => ResourceModel.Create(repository, setting)).ToArray();
        if (resources.Select(resource => resource.Route).Distinct(StringComparer.Ordinal).Count() != resources.Length)
        {
            throw new InvalidOperationException("CRUD routes must be unique in the generator configuration.");
        }

        var files = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var resource in resources)
        {
            foreach (var (layer, relativePath, template) in templates)
            {
                var path = Path.Combine(
                    repository.GetFeatureDirectory(layer, resource.Feature),
                    relativePath.Replace("__Resource__", resource.Resource, StringComparison.Ordinal)
                        .Replace("__Feature__", resource.Feature, StringComparison.Ordinal));

                if (files.ContainsKey(path) || File.Exists(path) || Directory.Exists(path))
                {
                    throw new InvalidOperationException(
                        $"Cannot generate '{Path.GetRelativePath(repository.Root, path)}' because it already exists or is targeted more than once.");
                }

                var context = new TemplateContext
                {
                    StrictVariables = true,
                    EnableRelaxedMemberAccess = false
                };
                context.PushGlobal(new ScriptObject { ["model"] = resource });
                files.Add(path, template.Render(context).ReplaceLineEndings("\n").TrimEnd() + "\n");
            }
        }

        WriteFiles(files);
        return [.. files.Keys];
    }

    private static List<(string Layer, string RelativePath, Template Template)> LoadTemplates()
    {
        var directory = Path.Combine(AppContext.BaseDirectory, "Templates");
        var files = Directory.GetFiles(directory, "*.cs.sbn", SearchOption.AllDirectories);
        if (files.Length == 0)
        {
            throw new InvalidOperationException("The CRUD templates are missing.");
        }

        var templates = new List<(string Layer, string RelativePath, Template Template)>(files.Length);
        foreach (var file in files)
        {
            var relativePath = Path.GetRelativePath(directory, file);
            var separator = relativePath.IndexOf(Path.DirectorySeparatorChar);
            if (separator < 0)
            {
                throw new InvalidOperationException($"Template '{relativePath}' requires a layer directory.");
            }

            var template = Template.Parse(File.ReadAllText(file), relativePath);
            if (template.HasErrors)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, template.Messages));
            }

            templates.Add((relativePath[..separator], relativePath[(separator + 1)..^".sbn".Length], template));
        }

        return templates;
    }

    private static void WriteFiles(SortedDictionary<string, string> files)
    {
        var created = new List<string>(files.Count);
        try
        {
            foreach (var (path, source) in files)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)
                    ?? throw new InvalidOperationException($"Output file '{path}' has no directory."));
                using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                created.Add(path);
                using var writer = new StreamWriter(stream);
                writer.Write(source);
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            foreach (var path in created)
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception cleanupException) when (cleanupException is IOException or UnauthorizedAccessException)
                {
                    Console.Error.WriteLine($"Could not remove '{path}': {cleanupException.Message}");
                }
            }

            throw;
        }
    }
}
