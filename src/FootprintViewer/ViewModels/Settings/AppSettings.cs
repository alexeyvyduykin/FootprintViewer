using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FootprintViewer.ViewModels
{
    [DataContract]
    public class AppSettings : SidePanelTab//ReactiveObject
    {
        public AppSettings()
        {
            FootprintProvider = new ProviderSettings()
            {
                Type = ProviderType.Footprints,
                AvailableSources = new List<ISourceBuilder>()
                {
                    new RandomSourceBuilder("RandomFootprints"),
                    new DatabaseSourceBuilder(),
                }
            };

            GroundTargetProvider = new ProviderSettings()
            {
                Type = ProviderType.GroundTargets,
                AvailableSources = new List<ISourceBuilder>()
                {
                    new RandomSourceBuilder("RandomGroundTargets"),
                    new DatabaseSourceBuilder(),
                }
            };

            GroundStationProvider = new ProviderSettings()
            {
                Type = ProviderType.GroundStations,
                AvailableSources = new List<ISourceBuilder>()
                {
                    new RandomSourceBuilder("RandomGroundStations"),
                    new DatabaseSourceBuilder(),
                }
            };

            SatelliteProvider = new ProviderSettings()
            {
                Type = ProviderType.Satellites,
                AvailableSources = new List<ISourceBuilder>()
                {
                    new RandomSourceBuilder("RandomSatellites"),
                    new DatabaseSourceBuilder(),
                }
            };

            UserGeometryProvider = new ProviderSettings()
            {
                Type = ProviderType.UserGeometries,
                AvailableSources = new List<ISourceBuilder>()
                {
                    new DatabaseSourceBuilder(),
                }
            };

            FootprintPreviewGeometryProvider = new ProviderSettings()
            {
                Type = ProviderType.FootprintPreviewGeometries,
                AvailableSources = new List<ISourceBuilder>()
                {
                    new FileSourceBuilder("Shapefile", "shp"),
                }
            };

            MapBackgroundProvider = new ProviderSettings()
            {
                Type = ProviderType.MapBackgrounds,
                AvailableSources = new List<ISourceBuilder>()
                {
                    new FolderSourceBuilder("*.mbtiles"),
                }
            };

            FootprintPreviewProvider = new ProviderSettings()
            {
                Type = ProviderType.FootprintPreviews,
                AvailableSources = new List<ISourceBuilder>()
                {
                    new FolderSourceBuilder("*.mbtiles"),
                }
            };

            Title = "Пользовательские настройки";
        }

        [DataMember]
        public string? LastOpenDirectory { get; set; }

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
