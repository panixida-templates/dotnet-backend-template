using System.Reflection;

namespace Organization.Product.CodeGenerator;

internal sealed record RepositoryLayout(
    string Root,
    string ModuleName,
    string ModuleNamespace,
    Assembly Domain)
{
    internal static RepositoryLayout Find(string startDirectory, Assembly domain)
    {
        var moduleNamespace = (domain.GetName().Name
            ?? throw new InvalidOperationException("The Domain assembly has no name."))[..^".Domain".Length];
        var moduleName = moduleNamespace[(moduleNamespace.LastIndexOf('.') + 1)..];
        for (var directory = new DirectoryInfo(startDirectory); directory is not null; directory = directory.Parent)
        {
            var modulePath = Path.Combine(directory.FullName, "src", "Modules", moduleName);
            if (File.Exists(Path.Combine(modulePath, $"{moduleNamespace}.Domain", $"{moduleNamespace}.Domain.csproj")))
            {
                return new RepositoryLayout(directory.FullName, moduleName, moduleNamespace, domain);
            }
        }

        throw new InvalidOperationException($"Could not find the source projects for module '{moduleNamespace}'.");
    }

    internal string GetFeatureDirectory(string layer, string feature)
    {
        var projectName = $"{ModuleNamespace}.{layer}";
        var projectPath = Path.Combine(Root, "src", "Modules", ModuleName, projectName);
        if (!File.Exists(Path.Combine(projectPath, $"{projectName}.csproj")))
        {
            throw new InvalidOperationException($"Project '{projectName}' was not found in '{projectPath}'.");
        }

        return layer switch
        {
            "Domain" or "Application" => Path.Combine(projectPath, feature),
            "Infrastructure" => Path.Combine(projectPath, "Persistence", "Features", feature),
            "Presentation" => Path.Combine(projectPath, "Features", feature),
            _ => throw new InvalidOperationException($"Unknown template layer '{layer}'.")
        };
    }
}
