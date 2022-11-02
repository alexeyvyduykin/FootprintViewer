using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class FootprintPreviewTab : SidePanelTab
    {
        private readonly Data.DataManager.IDataManager _dataManager;
        private readonly SourceList<FootprintPreviewViewModel> _footprintPreviews = new();
        private readonly ReadOnlyObservableCollection<FootprintPreviewViewModel> _items;
        private readonly IProvider<(string, Geometry)> _footprintPreviewGeometryProvider;
        private readonly ObservableAsPropertyHelper<IDictionary<string, Geometry>> _geometries;
        private readonly ObservableAsPropertyHelper<bool> _isLoading;

        public FootprintPreviewTab(IReadonlyDependencyResolver dependencyResolver)
        {
            _dataManager = dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();

            _footprintPreviewGeometryProvider = dependencyResolver.GetExistingService<IProvider<(string, Geometry)>>();

            Filter = new FootprintPreviewTabFilter(dependencyResolver);

            Title = "Поиск сцены";

            _footprintPreviews
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Filter(Filter.FilterObservable)
                .Bind(out _items)
                .Subscribe();

            Loading = ReactiveCommand.CreateFromTask(LoadingImpl);

            LoadingGeometries = ReactiveCommand.CreateFromTask(LoadingGeometriesImpl);

            Delay = ReactiveCommand.CreateFromTask(() => Task.Delay(TimeSpan.FromSeconds(1.5)));

            Enter = ReactiveCommand.Create<FootprintPreviewViewModel, FootprintPreviewViewModel>(s => s);

            Leave = ReactiveCommand.Create(() => { });

            _isLoading = Delay.IsExecuting
                              .ObserveOn(RxApp.MainThreadScheduler)
                              .ToProperty(this, x => x.IsLoading);

            // TODO: duplicates           
            _geometries = LoadingGeometries
                .Select(s => s.ToDictionary(s => s.Item1, s => s.Item2))
                .ToProperty(this, x => x.Geometries);

            // TODO: remove this
            LoadingGeometries.Execute().Subscribe();

            // First loading
            //this.WhenAnyValue(s => s.IsActive)
            //    .Take(1)
            //    .Where(active => active == true)
            //    .Select(_ => Unit.Default)
            //    .InvokeCommand(LoadFootprintPreviewGeometry);

            this.WhenAnyValue(s => s.IsActive)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(active => active == true)
                //.Take(1)
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

        private ReactiveCommand<Unit, List<(string, Geometry)>> LoadingGeometries { get; }

        public ReactiveCommand<FootprintPreviewViewModel, FootprintPreviewViewModel> Enter { get; }

        public ReactiveCommand<Unit, Unit> Leave { get; }

        public bool IsLoading => _isLoading.Value;

        private async Task LoadingImpl()
        {
            var res = await _dataManager.GetDataAsync<FootprintPreview>(DbKeys.FootprintPreviews.ToString());
            var list = res.Select(s => new FootprintPreviewViewModel(s)).ToList();

            _footprintPreviews.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(list);
            });
        }

        private async Task<List<(string, Geometry)>> LoadingGeometriesImpl()
        {
            return await _footprintPreviewGeometryProvider.GetNativeValuesAsync(null);
        }

        public void SetAOI(Geometry aoi) => ((FootprintPreviewTabFilter)Filter).AOI = aoi;

        public void ResetAOI() => ((FootprintPreviewTabFilter)Filter).AOI = null;

        public IFilter<FootprintPreviewViewModel> Filter { get; }

        public IDictionary<string, Geometry> Geometries => _geometries.Value;

        public IObservable<FootprintPreviewViewModel?> SelectedItemObservable => this.WhenAnyValue(s => s.SelectedItem);

        public ReadOnlyObservableCollection<FootprintPreviewViewModel> Items => _items;

        [Reactive]
        public FootprintPreviewViewModel? SelectedItem { get; set; }
    }
}
