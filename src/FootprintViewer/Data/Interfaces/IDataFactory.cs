using NetTopologySuite.Geometries;

namespace FootprintViewer.Data;

public interface IDataFactory
{
    FootprintViewer.Data.DataManager.IDataManager CreateDataManager();

    IProvider<GroundStation> CreateGroundStationProvider();

    IProvider<GroundTarget> CreateGroundTargetProvider();

    IProvider<Footprint> CreateFootprintProvider();

    IProvider<Satellite> CreateSatelliteProvider();

    IEditableProvider<UserGeometry> CreateUserGeometryProvider();

    IProvider<(string, Geometry)> CreateFootprintPreviewGeometryProvider();

    IProvider<MapResource> CreateMapBackgroundProvider();

    IProvider<FootprintPreview> CreateFootprintPreviewProvider();
}
