using System.Reflection;
using System.Text;

using Scriban;
using Scriban.Runtime;

namespace Organization.Product.CodeGenerator;

internal static class CrudGenerator
{
    internal static async Task<IReadOnlyList<OutputChange>> RunJobsAsync(
        RepositoryLayout repository,
        IReadOnlyList<JobSettings> jobs)
    {
        var catalog = LoadCatalog();
        var parsedTemplates = new Dictionary<string, Template>(StringComparer.Ordinal);
        var domains = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var routes = new Dictionary<string, string>(StringComparer.Ordinal);
        var changes = new List<OutputChange>();
        foreach (var job in jobs)
        {
            ValidateJob(job, catalog);
            var project = DomainProject.Find(repository, job.PathToModels);
            var assembly = job.DeleteGenerated ? null : await GetDomainAsync(repository, project, domains);
            foreach (var settings in job.Models)
            {
                if (settings is null)
                {
                    throw new InvalidOperationException("The models array cannot contain null entries.");
                }

                var target = project.Locate(settings.Aggregate);
                var model = assembly is null ? null : ResourceModel.Create(assembly, target, settings);
                if (model is not null && job.GeneratedFiles.Contains("Endpoints"))
                {
                    RegisterRoute(routes, model.Route, target.Aggregate);
                }

                foreach (var artifactName in job.GeneratedFiles)
                {
                    var artifact = catalog[artifactName];
                    var path = GetOutputPath(repository, target, job, artifact);
                    if (!paths.Add(path) || Directory.Exists(path))
                    {
                        throw new InvalidOperationException(
                            $"'{RelativePath(repository, path)}' is not a unique file path.");
                    }

                    var change = PlanChange(repository, target, model, job, artifact, path, parsedTemplates);
                    if (change is not null)
                    {
                        changes.Add(change);
                    }
                }
            }
        }

        OutputChange.ApplyAll(changes);
        return changes;
    }

    private static async Task<Assembly> GetDomainAsync(
        RepositoryLayout repository,
        DomainProject project,
        Dictionary<string, Assembly> domains)
    {
        if (!domains.TryGetValue(project.ProjectFile, out var assembly))
        {
            assembly = await project.BuildAsync(repository.Root);
            domains.Add(project.ProjectFile, assembly);
        }

        return assembly;
    }

    private static void ValidateJob(JobSettings? job, IReadOnlyDictionary<string, TemplateArtifact> catalog)
    {
        if (job is null
            || job.Models is not { Count: > 0 }
            || job.GeneratedFiles is not { Count: > 0 })
        {
            throw new InvalidOperationException("Each job requires nonempty models and generatedFiles arrays.");
        }

        if (job.OverwriteExisting && job.DeleteGenerated)
        {
            throw new InvalidOperationException(
                "overwriteExisting and deleteGenerated cannot both be true in one job.");
        }

        if (job.Templates?.Values.Any(string.IsNullOrWhiteSpace) == true
            || job.OutputDirectory?.Values.Any(string.IsNullOrWhiteSpace) == true)
        {
            throw new InvalidOperationException("Template and output directory overrides must be nonempty paths.");
        }

        var names = job.GeneratedFiles.Concat(job.Templates?.Keys ?? []).Concat(job.OutputDirectory?.Keys ?? []);
        foreach (var name in names)
        {
            if (name is null || !catalog.ContainsKey(name))
            {
                throw new InvalidOperationException($"Unknown generated file '{name}'.");
            }
        }
    }

    private static void RegisterRoute(Dictionary<string, string> routes, string route, string aggregate)
    {
        if (!routes.TryAdd(route, aggregate) && routes[route] != aggregate)
        {
            throw new InvalidOperationException($"The route '{route}' is configured for more than one aggregate.");
        }
    }

    private static string GetOutputPath(
        RepositoryLayout repository,
        CrudTarget target,
        JobSettings job,
        TemplateArtifact artifact)
    {
        var file = artifact.RelativePath.Replace("__Resource__", target.Resource, StringComparison.Ordinal)
            .Replace("__Feature__", target.Feature, StringComparison.Ordinal);
        if (job.OutputDirectory?.TryGetValue(artifact.Name, out var configuredDirectory) == true)
        {
            var expanded = target.ExpandOutputDirectory(configuredDirectory);
            var destination = repository.ResolvePath(expanded, repository.Root);
            return repository.ResolvePath(Path.Combine(destination, Path.GetFileName(file)), repository.Root);
        }

        return repository.ResolvePath(Path.Combine(target.DefaultDirectory(artifact.Layer), file), repository.Root);
    }

    private static OutputChange? PlanChange(
        RepositoryLayout repository,
        CrudTarget target,
        ResourceModel? model,
        JobSettings job,
        TemplateArtifact artifact,
        string path,
        Dictionary<string, Template> parsedTemplates)
    {
        var previous = File.Exists(path) ? File.ReadAllBytes(path) : null;
        var label = RelativePath(repository, path);
        var header = $"// Scaffolded by CodeGenerator for {target.Aggregate}.";
        if (job.DeleteGenerated)
        {
            if (previous is null)
            {
                return null;
            }

            var firstLine = Encoding.UTF8.GetString(previous).Split('\n', 2)[0].Trim('\uFEFF', '\r');
            if (firstLine != header)
            {
                throw new InvalidOperationException($"Cannot delete '{label}': its scaffold marker does not match.");
            }

            return new OutputChange(path, previous, null);
        }

        if (previous is not null && !job.OverwriteExisting)
        {
            throw new InvalidOperationException(
                $"Cannot replace '{label}': it already exists and overwriteExisting is false.");
        }

        var templatePath = job.Templates?.TryGetValue(artifact.Name, out var overridePath) == true
            ? repository.ResolvePath(overridePath, repository.GeneratorDirectory)
            : artifact.SourcePath;
        var source = Render(templatePath, model, parsedTemplates);
        return new OutputChange(path, previous, Encoding.UTF8.GetBytes($"{header}\n\n{source}"));
    }

    private static string Render(string path, ResourceModel? model, Dictionary<string, Template> templates)
    {
        if (!templates.TryGetValue(path, out var template))
        {
            template = Template.Parse(File.ReadAllText(path), path);
            if (template.HasErrors)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, template.Messages));
            }

            templates.Add(path, template);
        }

        var context = new TemplateContext
        {
            StrictVariables = true,
            EnableRelaxedMemberAccess = false
        };
        context.PushGlobal(new ScriptObject { ["model"] = model });
        return template.Render(context).ReplaceLineEndings("\n").TrimEnd() + "\n";
    }

    private static Dictionary<string, TemplateArtifact> LoadCatalog()
    {
        var directory = Path.Combine(AppContext.BaseDirectory, "Templates");
        var catalog = new Dictionary<string, TemplateArtifact>(StringComparer.Ordinal);
        foreach (var path in Directory.EnumerateFiles(directory, "*.cs.sbn", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(directory, path);
            var separator = relativePath.IndexOf(Path.DirectorySeparatorChar);
            if (separator < 0
                || relativePath[..separator] is not ("Domain" or "Application" or "Infrastructure" or "Presentation"))
            {
                continue;
            }

            var outputFile = relativePath[(separator + 1)..^".sbn".Length];
            var name = Path.GetFileNameWithoutExtension(outputFile)
                .Replace("__Resource__", string.Empty, StringComparison.Ordinal)
                .Replace("__Feature__", string.Empty, StringComparison.Ordinal);
            var artifact = new TemplateArtifact(name, relativePath[..separator], outputFile, path);
            if (!catalog.TryAdd(name, artifact))
            {
                throw new InvalidOperationException($"Duplicate artifact name '{name}' in the bundled templates.");
            }
        }

        return catalog;
    }

    private static string RelativePath(RepositoryLayout repository, string path) =>
        Path.GetRelativePath(repository.Root, path);

    private sealed record TemplateArtifact(string Name, string Layer, string RelativePath, string SourcePath);
}
