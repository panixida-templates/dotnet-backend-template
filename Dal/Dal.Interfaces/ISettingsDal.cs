using Common.ConvertParams;
using Common.SearchParams;

using Dal.Interfaces.Core;

using Entities;

namespace Dal.Interfaces;

public interface ISettingsDal : IBaseDal<int, Setting, SettingsSearchParams, SettingsConvertParams>
{
}
