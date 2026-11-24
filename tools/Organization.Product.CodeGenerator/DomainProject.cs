using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.RegularExpressions;

namespace Organization.Product.CodeGenerator;

internal sealed record DomainProject(string ProjectFile, string Module, string ModuleName)
{
    private const string DomainSuffix = ".Domain";

    internal static DomainProject Find(RepositoryLayout repository, string pathToModels)
    {
        var path = repository.ResolvePath(pathToModels, repository.Root);
        var projects = Directory.Exists(path)
            ? Directory.GetFiles(path, "*.Domain.csproj")
            : [path];
        if (projects.Length != 1
            || !projects[0].EndsWith(".Domain.csproj", StringComparison.Ordinal)
            || !File.Exists(projects[0]))
        {
            throw new InvalidOperationException($"'{pathToModels}' must identify exactly one .Domain.csproj.");
        }

        var projectFile = repository.ResolvePath(projects[0], repository.Root);
        var module = Path.GetFileNameWithoutExtension(projectFile)[..^DomainSuffix.Length];
        var moduleName = module[(module.LastIndexOf('.') + 1)..];
        return new DomainProject(projectFile, module, moduleName);
    }

    internal CrudTarget Locate(string aggregate)
    {
        var prefix = $"{Module}{DomainSuffix}.";
        if (!string.IsNullOrWhiteSpace(aggregate) && aggregate.StartsWith(prefix, StringComparison.Ordinal))
        {
            var names = aggregate[prefix.Length..].Split('.');
            if (names.Length == 2 && names.All(IsIdentifier))
            {
                return new CrudTarget(this, names[0], names[1]);
            }
        }

        throw new InvalidOperationException($"'{aggregate}' must match '{prefix}<Feature>.<Aggregate>'.");
    }

    internal async Task<Assembly> BuildAsync(string repositoryRoot)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = repositoryRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            ArgumentList =
            {
                "build",
                ProjectFile,
                "--nologo",
                "--verbosity",
                "quiet",
                "-target:Build",
                "-getProperty:TargetPath",
                "-p:CopyLocalLockFileAssemblies=true"
            }
        };

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException($"Could not build '{ProjectFile}'.");
        var output = process.StandardOutput.ReadToEndAsync();
        var errors = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        var standardOutput = await output;
        var standardError = await errors;
        if (process.ExitCode != 0)
        {
            var buildOutput = standardOutput + standardError;
            throw new InvalidOperationException($"Could not build '{ProjectFile}'.{Environment.NewLine}{buildOutput}");
        }

        const StringSplitOptions LineOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
        var targetPath = standardOutput.Split('\n', LineOptions)
            .LastOrDefault();
        if (targetPath is null || !Path.IsPathFullyQualified(targetPath) || !File.Exists(targetPath))
        {
            throw new InvalidOperationException($"Could not locate the build output of '{ProjectFile}'.");
        }

        return new DomainLoadContext(targetPath).LoadFromAssemblyPath(targetPath);
    }

    private static bool IsIdentifier(string name) =>
        Regex.IsMatch(name, @"^[A-Za-z_][A-Za-z0-9_]*\z", RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);

    private sealed class DomainLoadContext(string targetPath) : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver = new(targetPath);

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            var dependency = _resolver.ResolveAssemblyToPath(assemblyName);
            return dependency is null ? null : LoadFromAssemblyPath(dependency);
        }
    }
}
