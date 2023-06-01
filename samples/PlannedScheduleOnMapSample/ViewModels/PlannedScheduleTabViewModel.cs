using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using FootprintViewer.Helpers;
using Mapsui.Layers;
using PlannedScheduleOnMapSample.Layers;
using PlannedScheduleOnMapSample.ViewModels.Items;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PlannedScheduleOnMapSample.ViewModels;

public class PlannedScheduleTabViewModel : ViewModelBase
{
    private readonly SourceList<ITaskResult> _plannedSchedules = new();
    private readonly ReadOnlyObservableCollection<TaskResultViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private IObservable<IReadOnlyCollection<Footprint>> _layerObservable;

    public PlannedScheduleTabViewModel()
    {
        var observable = _plannedSchedules
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new TaskResultViewModel(s));

        observable
            .Sort(SortExpressionComparer<TaskResultViewModel>.Ascending(s => s.Begin))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe();

        _layerObservable = observable
            .Transform(s => TaskResultViewModel.Create(s.Model))
            .ToCollection();

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        _isLoading = Update.IsExecuting
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.IsLoading);

        Update.Execute().Subscribe();
    }

    public void ToLayerProvider(FootprintProvider provider)
    {
        provider.SetObservable(_layerObservable);
    }

    public void ToMemoryLayer(MemoryLayer layer)
    {
        _layerObservable.Subscribe(s =>
        {
            layer.Features = s.Select(s => FeatureBuilder.Build(s)).ToList();
        });
    }

    public void ToWritableLayer(WritableLayer layer)
    {
        _layerObservable.Subscribe(s =>
        {
            layer.AddRange(s.Select(s => FeatureBuilder.Build(s)).ToList());
            layer.DataHasChanged();
        });
    }

    public void ToObservableMemoryLayer(ObservableMemoryLayer<Footprint> layer)
    {
        var col = new ObservableCollection<Footprint>();

        layer.ObservableCollection = col;

        _layerObservable.Subscribe(s =>
        {
            layer.ObservableCollection.Add(s);
        });
    }

    public ReactiveCommand<Unit, Unit> Update { get; }

    public bool IsLoading => _isLoading.Value;

    private async Task UpdateImpl()
    {
        var list = await System.Reactive.Linq.Observable.Start(() =>
        {
            string path = System.IO.Path.Combine(EnvironmentHelpers.GetFullBaseDirectory(), "Assets", "PlannedSchedule.json");

            var result = (PlannedScheduleResult)JsonHelpers.DeserializeFromFile<PlannedScheduleResult>(path)!;

            return result.PlannedSchedules.ToList();
        }, RxApp.TaskpoolScheduler);

        _plannedSchedules.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(list);
        });
    }

    public ReadOnlyObservableCollection<TaskResultViewModel> Items => _items;
}
