using NetTopologySuite.Geometries;

namespace FootprintViewer.Data;

public interface IDataFactory
{
    FootprintViewer.Data.DataManager.IDataManager CreateDataManager();

    IEditableProvider<UserGeometry> CreateUserGeometryProvider();

    IProvider<(string, Geometry)> CreateFootprintPreviewGeometryProvider();
}
