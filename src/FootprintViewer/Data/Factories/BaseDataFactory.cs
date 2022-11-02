using FootprintViewer.Data.DataManager;
using FootprintViewer.Data.Managers;
using NetTopologySuite.Geometries;
using System;

namespace FootprintViewer.Data;

public abstract class BaseDataFactory : IDataFactory
{
    public abstract IDataManager CreateDataManager();

    public IEditableProvider<UserGeometry> CreateUserGeometryProvider()
    {
        var provider = new EditableProvider<UserGeometry>();

        provider.AddSources(GetUserGeometrySources());

        provider.AddManagers(new IEditableDataManager<UserGeometry>[]
        {
            new UserGeometryDataManager(),
        });

        return provider;
    }

    public IProvider<(string, Geometry)> CreateFootprintPreviewGeometryProvider()
    {
        var provider = new Provider<(string, Geometry)>();

        provider.AddSources(GetFootprintPreviewGeometrySources());

        provider.AddManagers(new IDataManager<(string, Geometry)>[]
        {
            new FootprintPreviewGeometryDataManager(),
        });

        return provider;
    }

    public IProvider<FootprintPreview> CreateFootprintPreviewProvider()
    {
        var provider = new Provider<FootprintPreview>();

        provider.AddSources(GetFootprintPreviewSources());

        provider.AddManagers(new IDataManager<FootprintPreview>[]
        {
            new FootprintPreviewDataManager(),
        });

        return provider;
    }

    protected virtual IDataSource[] GetFootprintPreviewSources()
    {
        return Array.Empty<IDataSource>();
    }

    protected virtual IDataSource[] GetFootprintPreviewGeometrySources()
    {
        return Array.Empty<IDataSource>();
    }

    protected virtual IDataSource[] GetUserGeometrySources()
    {
        return Array.Empty<IDataSource>();
    }
}
