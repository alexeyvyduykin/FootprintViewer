using System.Runtime.Serialization;

namespace DataSettingsSample.Data
{
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
    }
}
