using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Data.Models;
using PlannedScheduleOnMapSample.Design;
using PlannedScheduleOnMapSample.Layers;
using PlannedScheduleOnMapSample.ViewModels.Items;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace PlannedScheduleOnMapSample.ViewModels;

public class SatelliteTabViewModel : ViewModelBase
{
    private SatelliteProvider? _provider;
    private readonly SourceList<SatelliteViewModel> _satellites = new();
    private readonly ReadOnlyObservableCollection<SatelliteViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly DataManager _dataManager;
    private readonly Subject<PlannedScheduleResult> _subj = new();

    public SatelliteTabViewModel() : this(DesignData.CreateDataManager()) { }

    public SatelliteTabViewModel(DataManager dataManager)
    {
        _dataManager = dataManager;

        var observable = _satellites
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler);

        observable
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe();

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        _isLoading = Update.IsExecuting
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.IsLoading);

        Update.Execute().Subscribe();
    }

    public void ToLayerProvider(SatelliteProvider provider)
    {
        _provider = provider;

        var layerObservable = _subj.AsObservable();

        provider.SetObservable(layerObservable);
    }

    public ReactiveCommand<Unit, Unit> Update { get; }

    public bool IsLoading => _isLoading.Value;

    private async Task UpdateImpl()
    {
        var res = await _dataManager.GetDataAsync<PlannedScheduleResult>(MainWindowViewModel.PlannedScheduleKey);

        var satellites = res.FirstOrDefault()!.Satellites.Select(s => new SatelliteViewModel(s)).ToList();

        foreach (var item in satellites)
        {
            item.Observable.Subscribe(s => _provider?.Update(s.Name, s.Node, s.IsVisible));
        }

        _satellites.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(satellites);
        });

        _subj.OnNext(res.First());
    }

    public ReadOnlyObservableCollection<SatelliteViewModel> Items => _items;
}
