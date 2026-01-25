using Common.Enums;

using Dal.Ef.DbModels.Core;

namespace Dal.Ef.DbModels;

public sealed class SettingDbModel : BaseDbModel<int>
{
    public SettingType SettingType { get; set; }
    public string Value { get; set; } = string.Empty;
}
