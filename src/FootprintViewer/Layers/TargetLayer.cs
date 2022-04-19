using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using System;
using System.Reactive.Linq;

namespace FootprintViewer.Layers
{
    public class TargetLayer : BaseCustomLayer
    {
        private MRect _lastExtent = new MRect(1, 1, 1, 1);
        private readonly ITargetLayerSource _source;

        public TargetLayer(ITargetLayerSource source) : base(source)
        {
            _source = source;

            IsMapInfoLayer = false;
        }

        //public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType)
        public override void RefreshData(FetchInfo fetchInfo)
        {
            //base.RefreshData(extent, resolution, changeType);
            base.RefreshData(fetchInfo);

            var extent = fetchInfo.Extent;
            var resolution = fetchInfo.Resolution;

            if (fetchInfo.ChangeType == ChangeType.Discrete)
            {
                if (extent.Left != _lastExtent.Left && extent.Top != _lastExtent.Top && extent.Right != _lastExtent.Right && extent.Bottom != _lastExtent.Bottom)
                {
                    if (resolution < MaxVisible && extent.Equals(new MRect(0, 0, 0, 0)) == false)
                    {
                        // HACK: change size extent to viewport of view control
                        var box = extent.Grow(-SymbolStyle.DefaultWidth * 2 * resolution, -SymbolStyle.DefaultHeight * 2 * resolution);

                        var activeFeatures = GetFeatures(box, resolution);

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
