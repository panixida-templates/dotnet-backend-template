using Bl.Interfaces.Core;

using Dal.S3.Interfaces.Core;

using Dto.Storage;

namespace Bl.Implementations.Core;

public abstract class BaseStorageBl(IStorage storage) : IBaseStorageBl
{
    public Task<PresignedUrl> GetPresignedUploadUrlAsync(string fileName, string? contentType = null, TimeSpan? expiresIn = null)
    {
        return storage.GetPresignedUploadUrlAsync(fileName, contentType, expiresIn);
    }

    public Task<PresignedUrl> GetPresignedDownloadUrlAsync(string key, TimeSpan? expiresIn = null)
    {
        return storage.GetPresignedDownloadUrlAsync(key, expiresIn);
    }

    public Task<FileContent> DownloadAsync(string key, CancellationToken cancellationToken = default)
    {
        return storage.DownloadAsync(key, cancellationToken);
    }

    public Task<string> UploadAsync(FileContent fileContent, CancellationToken cancellationToken = default)
    {
        return storage.UploadAsync(fileContent, cancellationToken);
    }

    public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        return storage.DeleteAsync(key, cancellationToken);
    }

    public Task DeleteAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        return storage.DeleteAsync(keys, cancellationToken);
    }
}
