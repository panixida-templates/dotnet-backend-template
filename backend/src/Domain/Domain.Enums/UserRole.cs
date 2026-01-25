using System.ComponentModel.DataAnnotations;

namespace Domain.Enums;

public enum UserRole
{
    [Display(Name = "Разрабочтик")]
    Developer = 0,

    [Display(Name = "Администратор")]
    Admin = 1,

    [Display(Name = "Пользователь")]
    Client = 2,
}
