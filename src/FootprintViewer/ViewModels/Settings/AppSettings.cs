using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Runtime.Serialization;

namespace FootprintViewer.ViewModels
{
    [DataContract]
    public class AppSettings : SidePanelTab
    {
        public AppSettings()
        {
            FootprintProvider = CreateProviderSettings(
                ProviderType.Footprints,
                new ISourceBuilder[]
                {
                    new RandomSourceBuilder("RandomFootprints"),
                    new DatabaseSourceBuilder(this),
                });

            GroundTargetProvider = CreateProviderSettings(
                ProviderType.GroundTargets,
                new ISourceBuilder[]
                {
                    new RandomSourceBuilder("RandomGroundTargets"),
                    new DatabaseSourceBuilder(this),
                });

            GroundStationProvider = CreateProviderSettings(
                ProviderType.GroundStations,
                new ISourceBuilder[]
                {
                    new RandomSourceBuilder("RandomGroundStations"),
                    new DatabaseSourceBuilder(this),
                });

            SatelliteProvider = CreateProviderSettings(
                ProviderType.Satellites,
                new ISourceBuilder[]
                {
                    new RandomSourceBuilder("RandomSatellites"),
                    new DatabaseSourceBuilder(this),
                });

            UserGeometryProvider = CreateProviderSettings(
                ProviderType.UserGeometries,
                new ISourceBuilder[]
                {
                    new DatabaseSourceBuilder(this),
                });

            FootprintPreviewGeometryProvider = CreateProviderSettings(
                ProviderType.FootprintPreviewGeometries,
                new ISourceBuilder[]
                {
                    new FileSourceBuilder("Shapefile", "shp"),
                });

            MapBackgroundProvider = CreateProviderSettings(
                ProviderType.MapBackgrounds,
                new ISourceBuilder[]
                {
                    new FolderSourceBuilder("*.mbtiles"),
                });

            FootprintPreviewProvider = CreateProviderSettings(
                ProviderType.FootprintPreviews,
                new ISourceBuilder[]
                {
                    new FolderSourceBuilder("*.mbtiles"),
                });

            Title = "Пользовательские настройки";
        }

        private ProviderSettings CreateProviderSettings(ProviderType type, IEnumerable<ISourceBuilder> builders)
        {
            var settings = new ProviderSettings()
            {
                Type = type,
                AvailableSources = new List<ISourceBuilder>(builders)
            };

            settings.AddSource.Where(s => s is IDatabaseSourceInfo)
                              .Cast<IDatabaseSourceInfo>()
                              .Subscribe(s => LastDatabaseSource = s);

            settings.AddSource.Where(s => s is IFolderSourceInfo)
                              .Cast<IFolderSourceInfo>()
                              .Subscribe(s => LastOpenDirectory = s.Directory);

            settings.AddSource.Where(s => s is IFileSourceInfo)
                              .Cast<IFileSourceInfo>()
                              .Subscribe(s => LastOpenDirectory = System.IO.Path.GetDirectoryName(s.Path));

            return settings;
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
