using Mapsui;
using Mapsui.Extensions;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Layers;

public class DynamicLayer : BaseLayer, IAsyncDataFetcher, ILayerDataSource<IProvider>
{
    private readonly IProvider _dataSource;
    private FetchInfo? _fetchInfo;
    private List<IFeature> _features = new();
    private readonly bool _autoUpdatable;

    public DynamicLayer(IProvider dataSource, bool autoUpdatable = false)
    {
        _autoUpdatable = autoUpdatable;

        _dataSource = dataSource ?? throw new ArgumentException(null, nameof(dataSource));

        if (_dataSource is IDynamic dynamic)
        {
            dynamic.DataChanged += (s, e) =>
            {
                Catch.Exceptions(async () =>
                {
                    await UpdateDataAsync();
                    DataHasChanged();
                });
            };
        }
    }

    public async Task UpdateDataAsync()
    {
        if (_fetchInfo is null)
        {
            return;
        }

        var features = await _dataSource.GetFeaturesAsync(_fetchInfo);

        _features = features.ToList();

        OnDataChanged(new DataChangedEventArgs());
    }

    public override MRect? Extent => _dataSource.GetExtent();

    public override IEnumerable<IFeature> GetFeatures(MRect extent, double resolution)
    {
        return _features;
    }

    public void RefreshData(FetchInfo fetchInfo)
    {
        _fetchInfo = fetchInfo;

        if (_autoUpdatable == true)
        {
            if (DataSource is IDynamic dynamic)
            {
                dynamic.DataHasChanged();
            }
        }
    }

    public void AbortFetch()
    {
    }

    public void ClearCache()
    {
    }

    public IProvider? DataSource => _dataSource;
}
