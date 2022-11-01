using FootprintViewer.Data.DataManager;
using FootprintViewer.Data.Managers;
using NetTopologySuite.Geometries;
using System;

namespace FootprintViewer.Data;

public abstract class BaseDataFactory : IDataFactory
{
    public abstract IDataManager CreateDataManager();

    public IProvider<GroundTarget> CreateGroundTargetProvider()
    {
        var provider = new Provider<GroundTarget>();

        provider.AddSources(GetGroundTargetSources());

        provider.AddManagers(new IDataManager<GroundTarget>[]
        {
            new GroundTargetDataManager(),
            new RandomGroundTargetDataManager(),
        });

        return provider;
    }

    public IProvider<Footprint> CreateFootprintProvider()
    {
        var provider = new Provider<Footprint>();

        provider.AddSources(GetFootprintSources());

        provider.AddManagers(new IDataManager<Footprint>[]
        {
            new FootprintDataManager(),
            new RandomFootprintDataManager(),
        });

        return provider;
    }

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

    public IProvider<MapResource> CreateMapBackgroundProvider()
    {
        var provider = new Provider<MapResource>();

        provider.AddSources(GetMapBackgroundSources());

        provider.AddManagers(new IDataManager<MapResource>[]
        {
            new MapDataManager(),
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

    protected virtual IDataSource[] GetMapBackgroundSources()
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

    protected virtual IDataSource[] GetFootprintSources()
    {
        return Array.Empty<IDataSource>();
    }

    protected virtual IDataSource[] GetGroundTargetSources()
    {
        return Array.Empty<IDataSource>();
    }
}
