using FootprintViewer.Models;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.UI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;

namespace FootprintViewer.ViewModels
{
    public class SceneSearch : SidePanelTab
    {
        public event EventHandler CurrentFootprint;

        public SceneSearch()
        {
            this.WhenAnyValue(s => s.SelectedFootprint).Subscribe(footprint =>
            {
                if (footprint != null && Map != null && footprint.Path != null)
                {
                    var layer = MapsuiHelper.CreateMbTilesLayer(footprint.Path);

                    Map.Layers.Replace(nameof(LayerType.FootprintLayer), layer);

                    CurrentFootprint?.Invoke(this, EventArgs.Empty);
                }
            });

            MouseOverEnterCommand = ReactiveCommand.Create<Footprint>(ShowFootprintBorder);

            MouseOverLeaveCommand = ReactiveCommand.Create(HideFootprintBorder);

            SelectedItemChangedCommand = ReactiveCommand.Create<Footprint>(SelectionChanged);

            Footprints = new ObservableCollection<Footprint>();
        }

        public ReactiveCommand<Footprint, Unit> MouseOverEnterCommand { get; }

        public ReactiveCommand<Unit, Unit> MouseOverLeaveCommand { get; }

        public ReactiveCommand<Footprint, Unit> SelectedItemChangedCommand { get; }

        private void ShowFootprintBorder(Footprint footprint)
        {
            if (Map != null)
            {
                var layers = Map.Layers.FindLayer(nameof(LayerType.FootprintBorderLayer));

                if (layers != null)
                {
                    var layer = layers.SingleOrDefault();

                    if (layer != null && layer is WritableLayer writableLayer)
                    {
                        writableLayer.Clear();
                        writableLayer.Add(new Feature() { Geometry = footprint.Geometry });
                        writableLayer.DataHasChanged();
                    }
                }
            }
        }

        private void HideFootprintBorder()
        {
            if (Map != null)
            {
                var layers = Map.Layers.FindLayer(nameof(LayerType.FootprintBorderLayer));

                if (layers != null)
                {
                    var layer = layers.SingleOrDefault();

                    if (layer != null && layer is WritableLayer writableLayer)
                    {
                        writableLayer.Clear();
                        writableLayer.DataHasChanged();
                    }
                }
            }
        }

        private void SelectionChanged(Footprint footprint)
        {
            if (Map != null && footprint.Geometry != null)
            {
                var point = footprint.Geometry.BoundingBox.Centroid;

                Map.Initialized = false;

                Map.Home = (navigator) =>
                {
                    navigator.CenterOn(point);
                };

                // HACK: set Map.Initialized to false and add/remove layer for calling method CallHomeIfNeeded() and new initializing with Home
                var layer = new Mapsui.Layers.Layer();
                Map.Layers.Add(layer);
                Map.Layers.Remove(layer);
            }
        }

        [Reactive]
        public Map? Map { get; set; }

        [Reactive]
        public ObservableCollection<Footprint> Footprints { get; set; }

        [Reactive]
        public Footprint? SelectedFootprint { get; set; }
    }

    public class SceneSearchDesigner : SceneSearch
    {
        public SceneSearchDesigner() : base()
        {
            var f1 = new Footprint() { Name = "footprint1" };
            var f2 = new Footprint() { Name = "footprint2" };
            var f3 = new Footprint() { Name = "footprint3" };

            Footprints = new ObservableCollection<Footprint>(new[] { f1, f2, f3 });
        }
    }
}
