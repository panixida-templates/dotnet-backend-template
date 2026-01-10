using Bl.Implementations.Core;
using Bl.Interfaces;

using Common.ConvertParams;
using Common.SearchParams;

using Dal.Interfaces;

using Entities;

namespace Bl.Implementations;

public sealed class SettingsBl(ISettingsDal settingsDal) :
    BaseBl<int, Setting, SettingsSearchParams, SettingsConvertParams>(settingsDal),
    ISettingsBl
{
}
