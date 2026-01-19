using Common.ConvertParams;
using Common.SearchParams;

using Pl.Ui.Blazor.Admin.Pages.Core;
using Pl.Ui.Blazor.Services.Interfaces;
using Pl.Ui.Blazor.ViewModels;

namespace Pl.Ui.Blazor.Admin.Pages.Users;

public partial class UsersEdit : BaseEdit<Guid, UserViewModel, UsersSearchParams, UsersConvertParams, IUsersService>
{
    protected override string TableRoute => "/users";
    protected override string EditRoute => "/users/edit";
}
