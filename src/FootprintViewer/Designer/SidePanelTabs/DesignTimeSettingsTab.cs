using FootprintViewer.Configurations;
using FootprintViewer.Data;
using FootprintViewer.Data.Sources;
using FootprintViewer.Localization;
using FootprintViewer.ViewModels;
using Splat;
using System.Collections.Generic;

namespace FootprintViewer.Designer
{
    public class DesignTimeSettingsTab : SettingsTabViewModel
    {
        private static readonly IReadonlyDependencyResolver _resolver = new DesignTimeData();

        public DesignTimeSettingsTab() : base(_resolver)
        {
            //var groundStationProvider = new Provider<GroundStation>(new IDataSource[]
            //{
            //    new DatabaseSource() { Database = "FootprintViewerDatabase", Table = "GroundStations" },
            //    new RandomSource() { Name = "RandomGroundStations" },
            //});

            //var satelliteProvider = new Provider<Satellite>(new IDataSource[]
            //{
            //    new DatabaseSource() { Database = "FootprintViewerDatabase", Table = "Satellites" },
            //    new RandomSource() { Name = "RandomSatellites" },
            //});

            //var footprintProvider = new Provider<Footprint>(new IDataSource[]
            //{
            //    new DatabaseSource() { Database = "FootprintViewerDatabase", Table = "Footprints" },
            //    new RandomSource() { Name = "RandomFootprints" },
            //});

            //var groundTargetProvider = new Provider<GroundTarget>(new IDataSource[]
            //{
            //    new DatabaseSource() { Database = "FootprintViewerDatabase", Table = "GroundTargets" },
            //    new RandomSource() { Name = "RandomGroundTargets" },
            //});

            //var userGeometryProvider = new EditableProvider<UserGeometry>(new IDataSource[]
            //{
            //    new DatabaseSource() { Database = "FootprintViewerDatabase", Table = "UserGeometries" },
            //});

            //var mapBackgroundProvider = new Provider<MapResource>(new IDataSource[]
            //{
            //    new FolderSource() { Directory = @"\FootprintViewer\data\world" },
            //    new FolderSource() { Directory = @"\FootprintViewer\userData\world" },
            //});

            //var footprintPreviewProvider = new Provider<FootprintPreview>(new IDataSource[]
            //{
            //    new FileSource() { Path = @"\FootprintViewer\data\footprints" },
            //    new FileSource() { Path = @"\FootprintViewer\userData\footprints" },
            //});

            //var footprintPreviewGeometryProvider = new Provider<(string, NetTopologySuite.Geometries.Geometry)>(new IDataSource[]
            //{
            //    new FileSource() { Path = @"\FootprintViewer\data\mosaics-geotiff\mosaic-tiff-ruonly.shp" },
            //});

            //var groundStationProviderViewModel = new ProviderViewModel(groundStationProvider, _resolver)
            //{
            //    Type = ProviderType.GroundStations,
            //    AvailableBuilders = new[] { "random", "database" },
            //};

            //var satelliteProviderViewModel = new ProviderViewModel(satelliteProvider, _resolver)
            //{
            //    Type = ProviderType.Satellites,
            //    AvailableBuilders = new[] { "random", "database" },
            //};

            //var footprintProviderViewModel = new ProviderViewModel(footprintProvider, _resolver)
            //{
            //    Type = ProviderType.Footprints,
            //    AvailableBuilders = new[] { "random", "database" },
            //};

            //var groundTargetProviderViewModel = new ProviderViewModel(groundTargetProvider, _resolver)
            //{
            //    Type = ProviderType.GroundTargets,
            //    AvailableBuilders = new[] { "random", "database" },
            //};

            //var userGeometryProviderViewModel = new ProviderViewModel(userGeometryProvider, _resolver)
            //{
            //    Type = ProviderType.UserGeometries,
            //    AvailableBuilders = new[] { "database" },
            //};

            //var mapBackgroundProviderViewModel = new ProviderViewModel(mapBackgroundProvider, _resolver)
            //{
            //    Type = ProviderType.MapBackgrounds,
            //    AvailableBuilders = new[] { "folder" },
            //};

            //var footprintPreviewProviderViewModel = new ProviderViewModel(footprintPreviewProvider, _resolver)
            //{
            //    Type = ProviderType.FootprintPreviews,
            //    AvailableBuilders = new[] { "file" },
            //};

            //var footprintPreviewGeometryProviderViewModel = new ProviderViewModel(footprintPreviewGeometryProvider, _resolver)
            //{
            //    Type = ProviderType.FootprintPreviewGeometries,
            //    AvailableBuilders = new[] { "file" },
            //};

            //Providers = new List<ProviderViewModel>()
            //{
            //    footprintProviderViewModel,
            //    groundTargetProviderViewModel,
            //    groundStationProviderViewModel,
            //    satelliteProviderViewModel,
            //    userGeometryProviderViewModel,
            //    footprintPreviewGeometryProviderViewModel,
            //    mapBackgroundProviderViewModel,
            //    footprintPreviewProviderViewModel,
            //};

            var config = new LanguagesConfiguration() { AvailableLocales = new[] { "en", "ru" } };

            var languageManager = new LanguageManager(config);

            LanguageSettings = new LanguageSettingsViewModel(languageManager);

            LanguageSettings.Activate();
        }
    }
}
