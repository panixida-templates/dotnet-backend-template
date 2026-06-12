using System.ComponentModel.DataAnnotations;

namespace Organization.Product.Module.Presentation.Features.Users.Update;

public sealed record UpdateUserRequest(
    [property: Required]
    [property: StringLength(50)]
    string Role,

    [property: Required]
    [property: StringLength(200)]
    string Name,

    [property: Required]
    [property: EmailAddress]
    string Email,

    [property: Required]
    [property: Phone]
    string Phone,

    DateOnly BirthDate,

    [property: StringLength(500)]
    string? Avatar);
