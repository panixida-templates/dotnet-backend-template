namespace Organization.Product.CodeGenerator;

internal sealed record CrudTarget(DomainProject Project, string Feature, string Resource)
{
    public string Domain => $"{Project.Module}.Domain.{Feature}";

    public string Aggregate => $"{Domain}.{Resource}";

    internal string DefaultDirectory(string layer)
    {
        var moduleDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Project.ProjectFile))
            ?? throw new InvalidOperationException($"'{Project.ProjectFile}' has no module directory.");
        var projectName = $"{Project.Module}.{layer}";
        var projectDirectory = Path.Combine(moduleDirectory, projectName);
        if (layer == "Domain")
        {
            return Path.Combine(Path.GetDirectoryName(Project.ProjectFile) ?? projectDirectory, Feature);
        }

        if (!File.Exists(Path.Combine(projectDirectory, $"{projectName}.csproj")))
        {
            throw new InvalidOperationException($"Project '{projectName}' was not found in '{moduleDirectory}'.");
        }

        return layer switch
        {
            "Application" => Path.Combine(projectDirectory, Feature),
            "Infrastructure" => Path.Combine(projectDirectory, "Persistence", "Features", Feature),
            "Presentation" => Path.Combine(projectDirectory, "Features", Feature),
            _ => throw new InvalidOperationException($"Unknown template layer '{layer}'.")
        };
    }

    internal string ExpandOutputDirectory(string directory) =>
        directory.Replace("{Module}", Project.Module, StringComparison.Ordinal)
            .Replace("{ModuleName}", Project.ModuleName, StringComparison.Ordinal)
            .Replace("{Feature}", Feature, StringComparison.Ordinal)
            .Replace("{Resource}", Resource, StringComparison.Ordinal);
}
