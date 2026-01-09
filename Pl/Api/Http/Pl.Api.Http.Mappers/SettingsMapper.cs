using Entities;

using Pl.Api.Http.Dtos.Models;

using Riok.Mapperly.Abstractions;

namespace Pl.Api.Http.Mappers;

[Mapper]
public static partial class SettingsMapper
{
    public static partial Setting ToEntity(this SettingDto dto);
    public static partial List<Setting> ToEntity(this IEnumerable<SettingDto> dtos);
    public static partial SettingDto ToDto(this Setting entity);
    public static partial List<SettingDto> ToDto(this IEnumerable<Setting> entities);
}
