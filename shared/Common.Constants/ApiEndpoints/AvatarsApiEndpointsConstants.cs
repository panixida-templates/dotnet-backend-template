using Common.Constants.ApiEndpoints.Core;

namespace Common.Constants.ApiEndpoints;

public sealed class AvatarsApiEndpointsConstants : IBaseStorageApiEndpointsConstants<AvatarsApiEndpointsConstants>
{
    public const string BaseConstant = $"{IBaseApiRoutesConstants.BasePrefixConstant}/{VersionConstant}/{ResourceNameConstant}";

    public const string ResourceNameConstant = "avatars";

    public const string VersionConstant = ApiVersionsConstants.V1;

    public static string ResourceName => ResourceNameConstant;
    public static string Version => VersionConstant;
}
