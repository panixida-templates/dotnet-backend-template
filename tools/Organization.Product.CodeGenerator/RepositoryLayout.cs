namespace Organization.Product.CodeGenerator;

internal sealed record RepositoryLayout(string Root, string GeneratorDirectory)
{
    internal static RepositoryLayout Find(string startDirectory)
    {
        var projectName = typeof(RepositoryLayout).Assembly.GetName().Name
            ?? throw new InvalidOperationException("The generator assembly has no name.");
        for (var directory = new DirectoryInfo(startDirectory); directory is not null; directory = directory.Parent)
        {
            var generatorDirectory = Path.Combine(directory.FullName, "tools", projectName);
            if (File.Exists(Path.Combine(generatorDirectory, $"{projectName}.csproj")))
            {
                return new RepositoryLayout(directory.FullName, generatorDirectory);
            }
        }

        throw new InvalidOperationException("Could not find the repository containing the generator project.");
    }

    internal string ResolvePath(string path, string relativeTo)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new InvalidOperationException("Configured paths must be nonempty.");
        }

        var fullPath = Path.GetFullPath(path, relativeTo);
        var relativePath = Path.GetRelativePath(Root, fullPath);
        if (relativePath == ".."
            || relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
            || Path.IsPathRooted(relativePath))
        {
            throw new InvalidOperationException($"'{path}' must resolve inside the repository.");
        }

        EnsureNoLinks(fullPath, relativePath);
        return fullPath;
    }

    private static void EnsureNoLinks(string fullPath, string relativePath)
    {
        // Paths supplied in configuration must stay inside the repository on disk as well.
        var remaining = relativePath;
        var currentPath = fullPath;
        while (remaining != "." && remaining.Length > 0)
        {
            if ((File.Exists(currentPath) || Directory.Exists(currentPath))
                && File.GetAttributes(currentPath).HasFlag(FileAttributes.ReparsePoint))
            {
                throw new InvalidOperationException($"'{currentPath}' points to a symbolic link or junction.");
            }

            currentPath = Path.GetDirectoryName(currentPath) ?? string.Empty;
            remaining = Path.GetDirectoryName(remaining) ?? string.Empty;
        }
    }
}
