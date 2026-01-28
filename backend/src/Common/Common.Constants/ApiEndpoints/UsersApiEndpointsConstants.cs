using Common.Constants.ApiEndpoints.Core;

namespace Common.Constants.ApiEndpoints;

public sealed class UsersApiEndpointsConstants : IBaseApiEndpointsConstants<UsersApiEndpointsConstants, Guid>
{
    public const string BaseConstant = $"{IBaseApiRoutesConstants.BasePrefixConstant}/{VersionConstant}/{ResourceNameConstant}";

    public const string ResourceNameConstant = "users";
    public const string IdConstant = "{id:Guid}";

    public const string VersionConstant = ApiVersionsConstants.V1;

    public static string ResourceName => ResourceNameConstant;
    public static string Version => VersionConstant;
}
