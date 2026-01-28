using Common.SearchParams.Core;

using Domain.Enums;

namespace Common.SearchParams;

public sealed class UsersSearchParams : BaseSearchParams
{
    public UserRole? Role { get; set; }

    public UsersSearchParams() : base() { }
}