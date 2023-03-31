using System.Runtime.Serialization;

namespace FootprintViewer.Data.DbContexts;

public enum DbKeys
{
    [EnumMember(Value = "userGeometries")]
    UserGeometries,
    [EnumMember(Value = "maps")]
    Maps,
    [EnumMember(Value = "footprintPreviews")]
    FootprintPreviews,
    [EnumMember(Value = "footprintPreviewGeometries")]
    FootprintPreviewGeometries,
    [EnumMember(Value = "plannedSchedules")]
    PlannedSchedules,
}
