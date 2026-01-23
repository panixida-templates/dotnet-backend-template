namespace Dal.S3.Configurations;

public sealed class S3StorageOptions
{
    public required string ServiceUrl { get; set; }
    public required string BucketName { get; set; }
    public required string BasePrefix { get; set; }
    public required TimeSpan DefaultPresignTtl { get; set; }
}
