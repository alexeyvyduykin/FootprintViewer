using FootprintViewer.Data.DataManager;
using FootprintViewer.Data.Managers;
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

    protected virtual IDataSource[] GetUserGeometrySources()
    {
        return Array.Empty<IDataSource>();
    }
}
