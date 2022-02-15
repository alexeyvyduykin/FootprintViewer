using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.Layers
{
    public class TargetLayer : MemoryLayer
    {
        private readonly TargetLayerProvider _provider;
        private BoundingBox _lastExtent = new BoundingBox(1, 1, 1, 1);
        private IFeature? _lastSelected;
        private string[]? _names;

        public TargetLayer(TargetLayerProvider provider)
        {
            _provider = provider;

            DataSource = provider;

            Refresh = ReactiveCommand.Create<Unit, string[]?>(_ => _names);
        }

        public ReactiveCommand<Unit, string[]?> Refresh { get; }

        public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType)
        {
            base.RefreshData(extent, resolution, changeType);

            if (changeType == ChangeType.Discrete)
            {
                if (extent.Left != _lastExtent.Left && extent.Top != _lastExtent.Top && extent.Right != _lastExtent.Right && extent.Bottom != _lastExtent.Bottom)
                {
                    if (resolution < MaxVisible && extent.Equals(new BoundingBox(0, 0, 0, 0)) == false)
                    {
                        // HACK: change size extent to viewport of view control
                        var box = extent.Grow(-SymbolStyle.DefaultWidth * 2 * resolution, -SymbolStyle.DefaultHeight * 2 * resolution);

                        var activeFeatures = GetFeaturesInView(box, resolution);

                        _names = activeFeatures.Select(s => (string)s["Name"]).ToArray();

                        Debug.WriteLine("--- Start ---");

                        Refresh.Execute().Subscribe();

                        Debug.WriteLine("--- End ---");
                    }
                    else
                    {
                        _names = null;

                        Debug.WriteLine("--- Start ---");

                        Refresh.Execute().Subscribe();

                        Debug.WriteLine("--- End ---");
                    }

                    _lastExtent = extent.Copy();
                }
            }
        }

        public void SelectGroundTarget(string name)
        {
            var feature = _provider.FeaturesCache.Where(s => name.Equals((string)s["Name"])).First();

            if (_lastSelected != null)
            {
                _lastSelected["State"] = "Unselected";
            }

            feature["State"] = "Selected";

            _lastSelected = feature;

            DataHasChanged();
        }

        public void ShowHighlight(string name)
        {
            var feature = _provider.FeaturesCache.Where(s => name.Equals((string)s["Name"])).First();

            feature["Highlight"] = true;

            DataHasChanged();
        }

        public void HideHighlight()
        {
            _provider.FeaturesCache.ForEach(s => s["Highlight"] = false);

            DataHasChanged();
        }
    }
}
