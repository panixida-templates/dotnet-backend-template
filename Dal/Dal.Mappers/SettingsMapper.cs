using Dal.DbModels;
using Dal.Mappers.Core;

using Entities;

using Riok.Mapperly.Abstractions;

namespace Dal.Mappers;

[Mapper]
public sealed partial class SettingsMapper : IMapper<SettingDbModel, Setting>
{
    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    public static partial SettingDbModel ToDbModel(Setting entity);
    public static partial IEnumerable<SettingDbModel> ToDbModel(IEnumerable<Setting> entities);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial Setting ToEntity(SettingDbModel dbModel);
    public static partial IEnumerable<Setting> ToEntity(IEnumerable<SettingDbModel> dbModels);
}
