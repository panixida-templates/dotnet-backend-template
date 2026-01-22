using Amazon.S3;
using Amazon.S3.Transfer;

using Dal.S3.Configurations;
using Dal.S3.Implementations.Core;
using Dal.S3.Interfaces;

namespace Dal.S3.Implementations;

public sealed class AvatarsStorage(IAmazonS3 amazonS3, ITransferUtility transferUtility, S3StorageOptions options) :
    S3Storage(amazonS3, transferUtility, options, "avatars"),
    IAvatarsStorage
{
}
