﻿using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Layers;
using Mapsui;
using ReactiveUI;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class GroundStationTab : SidePanelTab
    {
        private readonly IProvider<GroundStation> _provider;
        private readonly SourceList<GroundStationViewModel> _groundStation = new();
        private readonly ReadOnlyObservableCollection<GroundStationViewModel> _items;
        private readonly ObservableAsPropertyHelper<bool> _isLoading;
        private readonly GroundStationLayer? _layer;

        public GroundStationTab(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<IProvider<GroundStation>>();
            var map = (Map)dependencyResolver.GetExistingService<IMap>();
            _layer = map.GetLayer<GroundStationLayer>(LayerType.GroundStation);

            Title = "Просмотр наземных станций";

            _groundStation
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _items)
                .Subscribe();

            Loading = ReactiveCommand.CreateFromTask(LoadingImpl);

            Delay = ReactiveCommand.CreateFromTask(() => Task.Delay(TimeSpan.FromSeconds(1.0)));

            _isLoading = Delay.IsExecuting.ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.IsLoading);

            this.WhenAnyValue(s => s.IsActive)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(active => active == true)
                .Take(1)
                .Select(_ => Unit.Default)
                .InvokeCommand(Loading);

            this.WhenAnyValue(s => s.IsActive)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(active => active == true)
                .Select(_ => Unit.Default)
                .InvokeCommand(Delay);
        }

        public ReactiveCommand<Unit, Unit> Loading { get; }

        private ReactiveCommand<Unit, Unit> Delay { get; }

        public bool IsLoading => _isLoading.Value;

        private async Task LoadingImpl()
        {
            var list = await _provider.GetValuesAsync(null, s => new GroundStationViewModel(s));

            foreach (var item in list)
            {
                item.UpdateObservable.Subscribe(s => _layer?.Update(s));
                item.ChangeObservable.Subscribe(s => _layer?.Update(s));
            }

            _groundStation.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(list);
            });
        }

        public ReadOnlyObservableCollection<GroundStationViewModel> Items => _items;
    }
}
