using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

using Common.Storage.Dtos;

using Dal.S3.Configurations;
using Dal.S3.Interfaces.Core;

namespace Dal.S3.Implementations.Core;

public abstract class S3Storage(IAmazonS3 amazonS3, ITransferUtility transferUtility, S3StorageOptions options, string storagePrefix) : IStorage
{
    public virtual Task<PresignedUrl> GetPresignedDownloadUrlAsync(string key, TimeSpan? expiresIn = null)
    {
        key = BuildKey(key);
        return GetPresignedUrlAsync(key, HttpVerb.GET, expiresIn: expiresIn);
    }

    public virtual Task<PresignedUrl> GetPresignedUploadUrlAsync(string fileName, string? contentType = null, TimeSpan? expiresIn = null)
    {
        var key = BuildKey(fileName);
        return GetPresignedUrlAsync(key, HttpVerb.PUT, contentType, expiresIn);
    }

    public virtual async Task<FileContent> DownloadAsync(string key, CancellationToken cancellationToken = default)
    {
        key = BuildKey(key);
        var request = new TransferUtilityOpenStreamRequest
        {
            BucketName = options.BucketName,
            Key = key,
        };
        var response = await transferUtility.OpenStreamWithResponseAsync(request, cancellationToken);

        var objectContent = new FileContent(response.ResponseStream, response.Headers.ContentType, Path.GetFileName(key));

        return objectContent;
    }

    public virtual async Task<string> UploadAsync(FileContent objectContent, CancellationToken cancellationToken = default)
    {
        var key = BuildKey(objectContent.FileName);

        var request = new TransferUtilityUploadRequest
        {
            BucketName = options.BucketName,
            Key = key,
            InputStream = objectContent.Content,
            ContentType = objectContent.ContentType,
        };
        await transferUtility.UploadWithResponseAsync(request, cancellationToken);

        return objectContent.FileName;
    }

    public virtual Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        key = BuildKey(key);
        var request = new DeleteObjectRequest
        {
            BucketName = options.BucketName,
            Key = key
        };

        return amazonS3.DeleteObjectAsync(request, cancellationToken);
    }

    public virtual async Task DeleteAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        const int maxKeysPerRequest = 1000;

        foreach (var batch in keys.Chunk(maxKeysPerRequest))
        {
            var request = new DeleteObjectsRequest
            {
                BucketName = options.BucketName,
                Quiet = true,
                Objects = [.. batch.Select(key => new KeyVersion { Key = BuildKey(key) })],
            };

            if (request.Objects.Count == 0)
            {
                continue;
            }

            await amazonS3.DeleteObjectsAsync(request, cancellationToken);
        }
    }

    protected virtual string BuildKey(string fileName)
    {
        fileName = Path.GetFileName(fileName);
        return $"{options.BasePrefix}/{storagePrefix}/{fileName}";
    }

    private async Task<PresignedUrl> GetPresignedUrlAsync(string key, HttpVerb verb, string? contentType = null, TimeSpan? expiresIn = null)
    {
        var protocol = options.ServiceUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            ? Protocol.HTTPS
            : Protocol.HTTP;
        var expiresAtUtc = DateTime.UtcNow.Add(expiresIn ?? options.DefaultPresignTtl);

        var request = new GetPreSignedUrlRequest
        {
            BucketName = options.BucketName,
            Key = key,
            Verb = verb,
            Expires = expiresAtUtc,
            Protocol = protocol,
        };

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            request.ContentType = contentType;
        }

        var url = await amazonS3.GetPreSignedURLAsync(request);

        return new PresignedUrl(key, url);
    }
}
