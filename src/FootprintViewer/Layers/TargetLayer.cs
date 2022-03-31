using Mapsui;
using Mapsui.Geometries;
using Mapsui.Providers;
using Mapsui.Styles;
using System;
using System.Reactive.Linq;

namespace FootprintViewer.Layers
{
    public class TargetLayer : BaseCustomLayer
    {
        private BoundingBox _lastExtent = new BoundingBox(1, 1, 1, 1);
        private readonly ITargetLayerSource _source;

        public TargetLayer(ITargetLayerSource source) : base(source)
        {
            _source = source;
        }

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

                        _source.ActiveFeatures = activeFeatures;

                        _source.Refresh.Execute().Subscribe();
                    }
                    else
                    {
                        _source.ActiveFeatures = null;

                        _source.Refresh.Execute().Subscribe();
                    }

                    _lastExtent = extent.Copy();
                }
            }
        }
    }
}
