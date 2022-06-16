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

        public void Init(ProjectFactory factory)
        {
            FootprintProvider.AvailableSources = new(factory.CreateFootprintProviderBuilders(FootprintProvider));

            GroundTargetProvider.AvailableSources = new(factory.CreateGroundTargetProviderBuilders(GroundTargetProvider));

            GroundStationProvider.AvailableSources = new(factory.CreateGroundStationProviderBuilders(GroundStationProvider));

            SatelliteProvider.AvailableSources = new(factory.CreateSatelliteProviderBuilders(SatelliteProvider));

            UserGeometryProvider.AvailableSources = new(factory.CreateUserGeometryProviderBuilders(UserGeometryProvider));

            FootprintPreviewGeometryProvider.AvailableSources = new(factory.CreateFootprintPreviewGeometryProviderBuilders(FootprintPreviewGeometryProvider));

            MapBackgroundProvider.AvailableSources = new(factory.CreateMapBackgroundProviderBuilders(MapBackgroundProvider));

            FootprintPreviewProvider.AvailableSources = new(factory.CreateFootprintPreviewProviderBuilders(FootprintPreviewProvider));

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
