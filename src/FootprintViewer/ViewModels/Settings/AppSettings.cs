using System.Runtime.Serialization;

namespace FootprintViewer.ViewModels
{
    [DataContract]
    public class AppSettings : SidePanelTab
    {
        public AppSettings()
        {
            FootprintProvider = new ProviderSettings() { Type = ProviderType.Footprints };

            GroundTargetProvider = new ProviderSettings() { Type = ProviderType.GroundTargets };

            GroundStationProvider = new ProviderSettings() { Type = ProviderType.GroundStations };

            SatelliteProvider = new ProviderSettings() { Type = ProviderType.Satellites };

            UserGeometryProvider = new ProviderSettings() { Type = ProviderType.UserGeometries };

            FootprintPreviewGeometryProvider = new ProviderSettings() { Type = ProviderType.FootprintPreviewGeometries };

            MapBackgroundProvider = new ProviderSettings() { Type = ProviderType.MapBackgrounds };

            FootprintPreviewProvider = new ProviderSettings() { Type = ProviderType.FootprintPreviews };

            Title = "Пользовательские настройки";
        }

        [DataMember]
        public string? LastOpenDirectory { get; set; }

        [DataMember]
        public IDatabaseSourceInfo? LastDatabaseSource { get; set; }

        [DataMember]
        public ProviderSettings FootprintProvider { get; private set; }

        [DataMember]
        public ProviderSettings GroundTargetProvider { get; private set; }

        [DataMember]
        public ProviderSettings GroundStationProvider { get; private set; }

        [DataMember]
        public ProviderSettings SatelliteProvider { get; private set; }

        [DataMember]
        public ProviderSettings UserGeometryProvider { get; private set; }

        [DataMember]
        public ProviderSettings FootprintPreviewGeometryProvider { get; private set; }

        [DataMember]
        public ProviderSettings MapBackgroundProvider { get; private set; }

        [DataMember]
        public ProviderSettings FootprintPreviewProvider { get; private set; }
    }
}
