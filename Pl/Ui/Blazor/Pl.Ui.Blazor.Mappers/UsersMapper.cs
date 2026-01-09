using Pl.Api.Http.Dtos.Models;

using Pl.Ui.Blazor.ViewModels;

using Riok.Mapperly.Abstractions;

namespace Pl.Ui.Blazor.Mappers;

[Mapper]
public static partial class UsersMapper
{
    public static partial UserViewModel ToViewModel(this UserDto dto);
    public static partial List<UserViewModel> ToViewModel(this IEnumerable<UserDto> dtos);
    public static partial UserDto ToDto(this UserViewModel viewModel);
    public static partial List<UserDto> ToDto(this IEnumerable<UserViewModel> viewModels);
}
