namespace Common.Constants.ApiEndpoints.Core;

public interface IBaseStorageApiEndpointsConstants<TEndpoint> : IBaseApiRoutesConstants
    where TEndpoint : IBaseStorageApiEndpointsConstants<TEndpoint>
{
    static virtual string Base() => $"{BasePrefixConstant}/{TEndpoint.Version}/{TEndpoint.ResourceName}";

    static virtual string PresignedUpload(string fileName, string? contentType)
    {
        var url = $"{TEndpoint.Base()}/{PresignedUploadConstant}?fileName={Uri.EscapeDataString(fileName)}";

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            url += $"&contentType={Uri.EscapeDataString(contentType)}";
        }

        return url;
    }
    static virtual string PresignedDownload(string key) => $"{TEndpoint.Base()}/{PresignedDownloadConstant}?key={Uri.EscapeDataString(key)}";

    static virtual string Download(string key) => $"{TEndpoint.Base()}/{DownloadConstant}/{Uri.EscapeDataString(key)}";
    static virtual string Upload() => $"{TEndpoint.Base()}/{UploadConstant}";

    static virtual string Delete(string key) => $"{TEndpoint.Base()}/{Uri.EscapeDataString(key)}";
    static virtual string DeleteBatch() => $"{TEndpoint.Base()}/{DeleteBatchConstant}";
}
