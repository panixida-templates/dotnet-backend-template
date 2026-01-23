namespace Common.Constants.ApiEndpoints.Core;

public interface IBaseApiRoutesConstants
{
    public const string BasePrefixConstant = "api";
    public const string GetByFilterSuffixConstant = "get-by-filter";

    private const string PresignedConstant = "presigned";
    public const string PresignedDownloadConstant = $"{PresignedConstant}/{DownloadConstant}";
    public const string PresignedUploadConstant = $"{PresignedConstant}/{UploadConstant}";
    public const string DownloadConstant = "download";
    public const string UploadConstant = "upload";
    public const string DownloadByKeyConstant = $"{DownloadConstant}/{KeyConstant}";
    public const string KeyConstant = "{*key}";
    public const string DeleteBatchConstant = "delete-batch";

    public static abstract string ResourceName { get; }
    public static abstract string Version { get; }
}
