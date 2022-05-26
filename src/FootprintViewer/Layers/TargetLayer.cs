using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace FootprintViewer.Layers
{
    public class TargetLayer : BaseCustomLayer<ITargetLayerSource>
    {
        private MRect _lastExtent = new(1, 1, 1, 1);

        public TargetLayer(ITargetLayerSource source) : base(source)
        {
            Refresh = ReactiveCommand.Create<IEnumerable<IFeature>?, IEnumerable<IFeature>?>(s => s);

            Refresh.Throttle(TimeSpan.FromSeconds(1))
                   .InvokeCommand(source.Refresh);
        }

        private readonly ReactiveCommand<IEnumerable<IFeature>?, IEnumerable<IFeature>?> Refresh;

        public override void RefreshData(FetchInfo fetchInfo)
        {
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

                        Refresh.Execute(activeFeatures).Subscribe();
                    }
                    else
                    {
                        Refresh.Execute(null).Subscribe();
                    }

                    _lastExtent = extent.Copy();
                }
            }
        }
    }
}
