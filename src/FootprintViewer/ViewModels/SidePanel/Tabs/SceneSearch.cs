﻿using FootprintViewer.Data;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class SceneSearchList : ViewerList<FootprintPreview>
    {
        public SceneSearchList(FootprintPreviewProvider provider) : base(provider)
        {

        }
    }

    public class SceneSearch : SidePanelTab
    {
        private readonly FootprintPreviewGeometryProvider _footprintPreviewGeometryProvider;
        private readonly Map _map;
        private bool _firstLoading = true;
        private readonly ObservableAsPropertyHelper<IDictionary<string, Geometry>> _geometries;
        public event EventHandler? CurrentFootprint;

        public SceneSearch(IReadonlyDependencyResolver dependencyResolver)
        {
            _map = dependencyResolver.GetExistingService<Map>();

            var footprintPreviewProvider = dependencyResolver.GetExistingService<FootprintPreviewProvider>();

            _footprintPreviewGeometryProvider = dependencyResolver.GetExistingService<FootprintPreviewGeometryProvider>();

            ViewerList = new SceneSearchList(footprintPreviewProvider);

            Title = "Поиск сцены";

            Filter = new SceneSearchFilter(dependencyResolver);

            ViewerList.SelectedItemObservable.Subscribe(s => SelectFootprint(s));
            ViewerList.MouseOverEnter.Subscribe(s => ShowFootprintBorder(s));
            ViewerList.MouseOverLeave.Subscribe(_ => HideFootprintBorder());

            FilterClick = ReactiveCommand.Create(FilterClickImpl);

            Filter.Update.InvokeCommand(ViewerList.Loading);

            ViewerList.Loading.Subscribe(_ => _firstLoading = false);

            LoadFootprintPreviewGeometry = ReactiveCommand.CreateFromTask(_footprintPreviewGeometryProvider.GetValuesAsync);

            // TODO: duplicates
            _geometries = LoadFootprintPreviewGeometry.Select(s => s.ToDictionary(s => s.Item1, s => s.Item2)).ToProperty(this, x => x.Geometries);

            // TODO: avoid from first loading design
            this.WhenAnyValue(s => s.IsActive)
                .Where(active => active == true && _firstLoading == true)
                .Select(_ => Filter)
                .InvokeCommand(ViewerList.Loading);

            this.WhenAnyValue(s => s.IsActive)
                .Where(active => active == true && _firstLoading == true)
                .Select(_ => Unit.Default)
                .InvokeCommand(LoadFootprintPreviewGeometry);

            this.WhenAnyValue(s => s.IsActive)
                .Where(active => active == true && _firstLoading == true)
                .Select(_ => Unit.Default)
                .InvokeCommand(Filter.Init);

            this.WhenAnyValue(s => s.IsActive).Where(active => active == false).Subscribe(_ => IsFilterOpen = false);

            this.WhenAnyValue(s => s.IsExpanded).Where(c => c == false).Subscribe(_ => IsFilterOpen = false);
        }

        public void SetAOI(Geometry aoi) => ((SceneSearchFilter)Filter).AOI = aoi;

        public void ResetAOI() => ((SceneSearchFilter)Filter).AOI = null;

        public ReactiveCommand<Unit, Unit> FilterClick { get; }

        private ReactiveCommand<Unit, List<(string, Geometry)>> LoadFootprintPreviewGeometry { get; }

        private void ShowFootprintBorder(FootprintPreview footprint)
        {
            if (IsGeometry(footprint) == true)
            {
                var layer = _map.GetLayer(LayerType.FootprintImageBorder);

                if (layer != null && layer is WritableLayer writableLayer)
                {
                    writableLayer.Clear();
                    writableLayer.Add(new GeometryFeature() { Geometry = ToGeometry(footprint) });
                    writableLayer.DataHasChanged();
                }
            }
        }

        private void HideFootprintBorder()
        {
            var layer = _map.GetLayer(LayerType.FootprintImageBorder);

            if (layer != null && layer is WritableLayer writableLayer)
            {
                writableLayer.Clear();
                writableLayer.DataHasChanged();
            }
        }

        private void SelectFootprint(FootprintPreview? footprint)
        {
            if (footprint != null && _map != null && footprint.Path != null)
            {
                var layer = MapsuiHelper.CreateMbTilesLayer(footprint.Path);

                _map.ReplaceLayer(layer, LayerType.FootprintImage);

                CurrentFootprint?.Invoke(this, EventArgs.Empty);

                NavigateToCenter(footprint);
            }
        }

        private void NavigateToCenter(FootprintPreview footprint)
        {
            if (IsGeometry(footprint) == true)
            {
                var point = GetCenter(footprint);

                _map.Initialized = false;

                _map.Home = (navigator) =>
                {
                    navigator.CenterOn(point);
                };

                // HACK: set Map.Initialized to false and add/remove layer for calling method CallHomeIfNeeded() and new initializing with Home
                var layer = new Mapsui.Layers.Layer();
                _map.Layers.Add(layer);
                _map.Layers.Remove(layer);
            }
        }

        private bool IsGeometry(FootprintPreview footprint)
        {
            return Geometries.ContainsKey(footprint.Name!);
        }

        private Geometry ToGeometry(FootprintPreview footprint)
        {
            return Geometries[footprint.Name!];
        }

        private MPoint GetCenter(FootprintPreview footprint)
        {
            return Geometries[footprint.Name!].Centroid.ToMPoint();// BoundingBox.Centroid;
        }

        private void FilterClickImpl()
        {
            IsFilterOpen = !IsFilterOpen;
        }

        [Reactive]
        public IViewerList<FootprintPreview> ViewerList { get; private set; }

        [Reactive]
        public IFilter<FootprintPreview> Filter { get; set; }

        [Reactive]
        public bool IsFilterOpen { get; private set; }

        private IDictionary<string, Geometry> Geometries => _geometries.Value;
    }
}
