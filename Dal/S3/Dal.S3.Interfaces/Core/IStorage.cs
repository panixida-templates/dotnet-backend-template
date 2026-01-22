using Dto.Storage;

namespace Dal.S3.Interfaces.Core;

public interface IStorage
{
    Task<PresignedUrl> GetPresignedUploadUrlAsync(string fileName, string? contentType = null, TimeSpan? expiresIn = null);
    Task<PresignedUrl> GetPresignedDownloadUrlAsync(string key, TimeSpan? expiresIn = null);
    Task<FileContent> DownloadAsync(string key, CancellationToken cancellationToken = default);
    Task<string> UploadAsync(FileContent objectContent, CancellationToken cancellationToken = default);
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);
    Task DeleteAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);
}
