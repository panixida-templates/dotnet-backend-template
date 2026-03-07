using Domain.Enums;

using Presentation.Api.Http.Dtos.Models.Core;

namespace Presentation.Api.Http.Dtos.Models;

public sealed record UserDto : BaseDto<Guid>
{
    public UserRole Role { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateOnly Birthday { get; set; }
    public string? AvatarKey { get; set; }
}
