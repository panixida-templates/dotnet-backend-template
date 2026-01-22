using Dto.Storage;

namespace Bl.Interfaces.Core;

public interface IBaseStorageBl
{
    Task<PresignedUrl> GetPresignedUploadUrlAsync(string fileName, string? contentType = null, TimeSpan? expiresIn = null);
    Task<PresignedUrl> GetPresignedDownloadUrlAsync(string key, TimeSpan? expiresIn = null);
    Task<FileContent> DownloadAsync(string key, CancellationToken cancellationToken = default);
    Task<string> UploadAsync(FileContent fileContent, CancellationToken cancellationToken = default);
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);
    Task DeleteAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);
}
