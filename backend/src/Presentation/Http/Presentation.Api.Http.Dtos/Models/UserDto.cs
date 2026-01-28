using Domain.Enums;

using Presentation.Api.Http.Dtos.Models.Core;

namespace Presentation.Api.Http.Dtos.Models;

public sealed record UserDto : BaseDto<Guid>
{
    public required UserRole Role { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required DateOnly Birthday { get; set; }
    public string? Avatar { get; set; }
}
