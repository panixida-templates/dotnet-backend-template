using Application.Abstractions.Queries;

namespace Application.Users.Search;

public sealed class UsersSearchParams : BaseSearchParams
{
    public string? Role { get; set; }

    public UsersSearchParams() : base() { }
}
