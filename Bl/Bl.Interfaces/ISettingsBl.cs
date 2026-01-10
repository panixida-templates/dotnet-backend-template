using Bl.Interfaces.Core;

using Common.ConvertParams;
using Common.SearchParams;

using Entities;

namespace Bl.Interfaces;

public interface ISettingsBl : IBaseBl<int, Setting, SettingsSearchParams, SettingsConvertParams>
{
}

