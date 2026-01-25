using Dal.Ef.DbModels;
using Dal.Ef.Mappers.Core;

using Entities;

using Riok.Mapperly.Abstractions;

namespace Dal.Ef.Mappers;

[Mapper]
public sealed partial class SettingsMapper : IMapper<int, SettingDbModel, Setting>
{
    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    public static partial void ToDbModel(Setting entity, SettingDbModel dbModel);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial Setting ToEntity(SettingDbModel dbModel);
}
