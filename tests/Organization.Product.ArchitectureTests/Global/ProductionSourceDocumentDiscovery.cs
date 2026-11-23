using Microsoft.CodeAnalysis;

namespace Organization.Product.ArchitectureTests.Global;

internal static class ProductionSourceDocumentDiscovery
{
    private const string SourceRootDirectoryName = "src";

    private static readonly string[] s_authorSourceRootDirectoryNames =
    [
        SourceRootDirectoryName,
        "tests",
        "tools"
    ];

    private static readonly Lazy<Task<ProductionSourceContext>> s_context =
        new(CreateContextAsync);

    private static readonly string[] s_productionSourceRootDirectoryNames =
    [
        SourceRootDirectoryName
    ];

    private static readonly string[] s_excludedDirectoryNames =
    [
        "bin",
        "obj",
        "Generated",
        "Migrations",
        ArchitectureDefinition.RepositoryExamplesDirectoryName
    ];

    internal static Task<T[]> GetAuthorItemsAsync<T>(
        Func<string, Document, Task<T[]>> getDocumentItemsAsync)
    {
        return GetItemsAsync(
            getDocumentItemsAsync,
            s_authorSourceRootDirectoryNames);
    }

    internal static Task<T[]> GetItemsAsync<T>(
        Func<string, Document, Task<T[]>> getDocumentItemsAsync)
    {
        return GetItemsAsync(
            getDocumentItemsAsync,
            s_productionSourceRootDirectoryNames);
    }

    private static async Task<T[]> GetItemsAsync<T>(
        Func<string, Document, Task<T[]>> getDocumentItemsAsync,
        IReadOnlyCollection<string> sourceRootDirectoryNames)
    {
        var context = await s_context.Value;
        var items = new List<T>();

        foreach (var project in context.Solution.Projects
            .Where(project =>
                IsSourceProject(
                    context.RepositoryRoot,
                    project.FilePath,
                    sourceRootDirectoryNames))
            .OrderBy(
                project => project.FilePath,
                StringComparer.Ordinal))
        {
            foreach (var document in project.Documents
                .Where(document =>
                    IsSourceFile(
                        context.RepositoryRoot,
                        document.FilePath,
                        sourceRootDirectoryNames))
                .OrderBy(
                    document => document.FilePath,
                    StringComparer.Ordinal))
            {
                items.AddRange(
                    await getDocumentItemsAsync(
                        context.RepositoryRoot,
                        document));
            }
        }

        return [.. items];
    }

    private static async Task<ProductionSourceContext> CreateContextAsync()
    {
        var repositoryRoot = FindRepositoryRoot();
        var solutionPaths = GetSolutionPaths(repositoryRoot).ToArray();
        var solutionPath = solutionPaths.SingleOrDefault(path => Path.GetExtension(path) == ".slnx")
            ?? solutionPaths.Single();
        var solution = await ProductionSourceWorkspaceFixture.Workspace.OpenSolutionAsync(solutionPath);

        return new ProductionSourceContext(
            RepositoryRoot: repositoryRoot,
            Solution: solution);
    }

    private static bool IsSourceProject(
        string repositoryRoot,
        string? projectFile,
        IReadOnlyCollection<string> sourceRootDirectoryNames)
    {
        return projectFile is not null
            && IsWithinSourceRoots(
                repositoryRoot,
                projectFile,
                sourceRootDirectoryNames);
    }

    private static bool IsSourceFile(
        string repositoryRoot,
        string? sourceFile,
        IReadOnlyCollection<string> sourceRootDirectoryNames)
    {
        return sourceFile is not null
            && IsWithinSourceRoots(
                repositoryRoot,
                sourceFile,
                sourceRootDirectoryNames)
            && !sourceFile
                .Split(
                    [
                        Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar
                    ],
                    StringSplitOptions.RemoveEmptyEntries)
                .Any(segment =>
                    s_excludedDirectoryNames.Contains(
                        segment,
                        StringComparer.OrdinalIgnoreCase));
    }

    private static bool IsWithinSourceRoots(
        string repositoryRoot,
        string path,
        IReadOnlyCollection<string> sourceRootDirectoryNames)
    {
        var fullPath = Path.GetFullPath(path);

        return sourceRootDirectoryNames.Any(sourceRootDirectoryName =>
        {
            var sourceRoot = Path.Combine(
                repositoryRoot,
                sourceRootDirectoryName)
                + Path.DirectorySeparatorChar;

            return fullPath.StartsWith(
                sourceRoot,
                StringComparison.OrdinalIgnoreCase);
        });
    }

    private static IEnumerable<string> GetSolutionPaths(string directory)
    {
        return Directory.EnumerateFiles(directory)
            .Where(path => Path.GetExtension(path) is ".slnx" or ".sln");
    }

    private static string FindRepositoryRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory);
             directory is not null;
             directory = directory.Parent)
        {
            if (Directory.Exists(Path.Combine(
                directory.FullName,
                SourceRootDirectoryName))
                && GetSolutionPaths(directory.FullName).Any())
            {
                return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException(
            $"Could not find repository root containing "
                + $"'{SourceRootDirectoryName}' and a solution file.");
    }
}

internal sealed record ProductionSourceContext(
    string RepositoryRoot,
    Solution Solution);
