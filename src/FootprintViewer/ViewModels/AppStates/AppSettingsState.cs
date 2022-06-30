using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FootprintViewer.ViewModels
{
    [DataContract]
    public class AppSettingsState
    {
        public AppSettingsState()
        {
            FootprintProvider = new ProviderState() { Type = ProviderType.Footprints };

            GroundTargetProvider = new ProviderState() { Type = ProviderType.GroundTargets };

            GroundStationProvider = new ProviderState() { Type = ProviderType.GroundStations };

            SatelliteProvider = new ProviderState() { Type = ProviderType.Satellites };

            UserGeometryProvider = new ProviderState() { Type = ProviderType.UserGeometries };

            FootprintPreviewGeometryProvider = new ProviderState() { Type = ProviderType.FootprintPreviewGeometries };

            MapBackgroundProvider = new ProviderState() { Type = ProviderType.MapBackgrounds };

            FootprintPreviewProvider = new ProviderState() { Type = ProviderType.FootprintPreviews };
        }

        public List<ProviderState> GetProviderStates()
        {
            return new List<ProviderState>()
            {
            FootprintProvider,
            GroundTargetProvider,
            GroundStationProvider,
            SatelliteProvider,
            UserGeometryProvider,
            FootprintPreviewGeometryProvider,
            MapBackgroundProvider,
            FootprintPreviewProvider
            };
        }

        [DataMember]
        public string? LastOpenDirectory { get; set; }

        [DataMember]
        public IDatabaseSourceInfo? LastDatabaseSource { get; set; }

        [DataMember]
        public ProviderState FootprintProvider { get; private set; }

        [DataMember]
        public ProviderState GroundTargetProvider { get; private set; }

        [DataMember]
        public ProviderState GroundStationProvider { get; private set; }

        [DataMember]
        public ProviderState SatelliteProvider { get; private set; }

        [DataMember]
        public ProviderState UserGeometryProvider { get; private set; }

        [DataMember]
        public ProviderState FootprintPreviewGeometryProvider { get; private set; }

        [DataMember]
        public ProviderState MapBackgroundProvider { get; private set; }

        [DataMember]
        public ProviderState FootprintPreviewProvider { get; private set; }
    }
}
