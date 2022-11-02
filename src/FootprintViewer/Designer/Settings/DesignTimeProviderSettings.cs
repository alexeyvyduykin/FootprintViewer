using FootprintViewer.Data;
using FootprintViewer.Data.Sources;
using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeProviderSettings : ProviderViewModel
    {
        //private static readonly IDataSource _dataSource1 = new DatabaseSource() { Database = "FootprintViewerDatabase", Table = "Satellites" };
        //private static readonly IDataSource _dataSource2 = new FolderSource() { Directory = "C:/data" };
        //private static readonly IDataSource _dataSource3 = new FileSource() { Path = "C:/data/worldMap.mbtiles" };
        //private static readonly IDataSource _dataSource4 = new RandomSource() { Name = "SatelliteRandom" };

        //private static readonly IProvider _provider = new Provider<Satellite>(new IDataSource[]
        //{
        //    _dataSource1,       
        //    _dataSource2,       
        //    _dataSource3,       
        //    _dataSource4,
        //});

        public DesignTimeProviderSettings() : base(null/*_provider*/, new DesignTimeData())
        {
            Type = ProviderType.Satellites;

            AvailableBuilders = new[]
            {
                "random",
                "database",
            };
        }
    }
}
