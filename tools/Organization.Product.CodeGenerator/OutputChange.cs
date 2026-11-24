namespace Organization.Product.CodeGenerator;

internal sealed record OutputChange(string Path, byte[]? Before, byte[]? After)
{
    public string Action => After is null ? "Deleted" : Before is null ? "Created" : "Replaced";

    internal static void ApplyAll(IReadOnlyList<OutputChange> changes)
    {
        var applied = new List<OutputChange>(changes.Count);
        try
        {
            foreach (var change in changes)
            {
                change.Apply(change.Before, change.After);
                applied.Add(change);
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            for (var index = applied.Count - 1; index >= 0; index--)
            {
                var change = applied[index];
                try
                {
                    change.Apply(change.After, change.Before);
                }
                catch (Exception rollbackError) when (rollbackError is IOException or UnauthorizedAccessException)
                {
                    Console.Error.WriteLine($"Could not restore '{change.Path}': {rollbackError.Message}");
                }
            }

            throw;
        }
    }

    private void Apply(byte[]? expected, byte[]? next)
    {
        var current = File.Exists(Path) ? File.ReadAllBytes(Path) : null;
        if ((expected is null) != (current is null)
            || (expected is not null && !expected.AsSpan().SequenceEqual(current)))
        {
            throw new IOException($"'{Path}' changed while the generation plan was being applied.");
        }

        if (next is null)
        {
            File.Delete(Path);
            return;
        }

        var directory = System.IO.Path.GetDirectoryName(Path)
            ?? throw new IOException($"'{Path}' has no output directory.");
        Directory.CreateDirectory(directory);
        var temporary = System.IO.Path.Combine(directory, $".codegenerator-{Guid.NewGuid():N}.tmp");
        try
        {
            using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                stream.Write(next);
            }

            File.Move(temporary, Path, overwrite: expected is not null);
        }
        finally
        {
            if (File.Exists(temporary))
            {
                File.Delete(temporary);
            }
        }
    }
}
