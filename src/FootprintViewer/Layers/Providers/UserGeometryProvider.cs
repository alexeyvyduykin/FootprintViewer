using DynamicData;
using FootprintViewer.Data.Models;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Layers.Providers;

public class UserGeometryProvider : IProvider, IDynamic, IFeatureProvider
{
    private readonly SourceList<UserGeometry> _userGeometries = new();
    private readonly ReadOnlyObservableCollection<IFeature> _features;

    public UserGeometryProvider(LayerStyleManager styleManager)
    {
        _userGeometries
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => (IFeature)s.Geometry!.ToFeature(s.Name!))
            .Bind(out _features)
            .Subscribe(_ => DataHasChanged());

        //Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        //_dataManager.DataChanged
        //    .Where(s => s.Contains(DbKeys.UserGeometries.ToString()))
        //    .ToSignal()
        //    .InvokeCommand(Update);

        //Observable.StartAsync(UpdateImpl);
    }

    public string? CRS { get; set; }

    public ReadOnlyObservableCollection<IFeature> Features => _features;

    //public ReactiveCommand<Unit, Unit> Update { get; }

    public event DataChangedEventHandler? DataChanged;

    public void SetObservable(IObservable<IReadOnlyCollection<UserGeometry>> observable)
    {
        observable.Subscribe(UpdateData);
    }

    private void UpdateData(IReadOnlyCollection<UserGeometry> userGeometries)
    {
        var list = userGeometries
            .Where(s => s.Geometry != null && string.IsNullOrEmpty(s.Name) == false)
            .ToList();

        _userGeometries.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(list);
        });
    }

    //private async Task UpdateImpl()
    //{
    //    var userGeometries = await _dataManager.GetDataAsync<UserGeometry>(DbKeys.UserGeometries.ToString());

    //    var list = userGeometries
    //        .Where(s => s.Geometry != null && string.IsNullOrEmpty(s.Name) == false)
    //        .ToList();

    //    _userGeometries.Edit(innerList =>
    //    {
    //        innerList.Clear();
    //        innerList.AddRange(list);
    //    });
    //}

    public virtual Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
    {
        if (fetchInfo == null || fetchInfo.Extent == null)
        {
            throw new Exception();
        }

        var source = Features.ToList();

        fetchInfo = new FetchInfo(fetchInfo);

        var resolution = fetchInfo.Resolution;
        var box = fetchInfo.Extent;
        var biggerBox = box.Grow(SymbolStyle.DefaultWidth * 2 * resolution, SymbolStyle.DefaultHeight * 2 * resolution);
        var source2 = source.Where(f => biggerBox.Intersects(f.Extent)).ToList();

        return Task.FromResult((IEnumerable<IFeature>)source2);
    }

    public MRect? GetExtent() => GetExtent(Features);

    private static MRect? GetExtent(IReadOnlyList<IFeature> features)
    {
        MRect? mRect = null;

        foreach (IFeature feature in features)
        {
            if (feature.Extent != null)
            {
                mRect = ((mRect == null) ? feature.Extent : mRect.Join(feature.Extent));
            }
        }

        return mRect;
    }

    public IFeature? Find(object? value, string fieldName)
    {
        object? value2 = value;
        string fieldName2 = fieldName;
        return Features.FirstOrDefault((IFeature f) => value2 != null && f[fieldName2] == value2);
    }

    public void DataHasChanged()
    {
        OnDataChanged();
    }

    private void OnDataChanged()
    {
        DataChanged?.Invoke(this, new DataChangedEventArgs(null, false, null));
    }
}
