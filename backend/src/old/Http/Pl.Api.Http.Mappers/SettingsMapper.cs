using Entities;

using Pl.Api.Http.Dtos.Models;
using Pl.Api.Http.Mappers.Core;

using Riok.Mapperly.Abstractions;

namespace Pl.Api.Http.Mappers;

[Mapper]
public sealed partial class SettingsMapper : IMapper<SettingDto, Setting>
{
    public static partial SettingDto ToDto(Setting entity);
    public static partial IEnumerable<SettingDto> ToDto(IEnumerable<Setting> entities);
    public static partial Setting ToEntity(SettingDto dto);
    public static partial IEnumerable<Setting> ToEntity(IEnumerable<SettingDto> dtos);
}
