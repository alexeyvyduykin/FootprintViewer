using FootprintViewer.Data;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.Layers
{
    public class TargetLayerEventArgs : EventArgs
    {
        public IEnumerable<IFeature>? Features { get; set; }
        public double Resolution { get; set; }
    }

    public delegate void TargetLayerEventHandler(object? sender, TargetLayerEventArgs e);

    public class TargetLayer : MemoryLayer
    {     
        private readonly TargetLayerProvider _provider; 
        private BoundingBox _lastExtent = new BoundingBox(1, 1, 1, 1);
        private IFeature? _lastSelected;
        private IEnumerable<IFeature>? _activeFeatures;

        public TargetLayer(TargetLayerProvider provider)
        {
            _provider = provider;
            
            DataSource = provider;
         
            Refresh = ReactiveCommand.Create<IEnumerable<IFeature>, IEnumerable<IFeature>>(s => s);

            _isEnabled = ReactiveCommand.Create<bool, bool>(s => IsEnable = s);
        }

        public IObservable<bool> IsEnabledObserver => _isEnabled;

        private ReactiveCommand<IEnumerable<IFeature>, IEnumerable<IFeature>> Refresh { get; }

        private readonly ReactiveCommand<bool, bool> _isEnabled;

        public bool IsEnable { get; private set; }

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

                        _activeFeatures = GetFeaturesInView(box, resolution);

                        Refresh.Execute(_activeFeatures).Subscribe();

                        _isEnabled.Execute(true).Subscribe();
                    }
                    else
                    {
                        _activeFeatures = null;

                        _isEnabled.Execute(false).Subscribe();
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

        public virtual IEnumerable<GroundTarget> GetTargets()
        {
            return _provider.FromDataSource(_activeFeatures);
        }


    }
}
