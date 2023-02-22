using System.Runtime.Serialization;

namespace FootprintViewer.Data.DataManager;

public enum DbKeys
{
    [EnumMember(Value = "footprints")]
    Footprints,
    [EnumMember(Value = "groundTargets")]
    GroundTargets,
    [EnumMember(Value = "satellites")]
    Satellites,
    [EnumMember(Value = "groundStations")]
    GroundStations,
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
