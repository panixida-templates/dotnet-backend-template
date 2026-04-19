using System.ComponentModel.DataAnnotations;

namespace Presentation.Http.Features.Users.Create;

public sealed record CreateUserRequest
{
    [Required]
    [StringLength(50)]
    public string Role { get; init; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [Phone]
    public string Phone { get; init; } = string.Empty;

    public DateOnly BirthDate { get; init; }

    [StringLength(500)]
    public string? Avatar { get; init; }
}
