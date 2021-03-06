using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using System.Collections.Generic;

namespace InteractiveGeometry
{
    public class InteractiveLayer : BaseLayer
    {
        private readonly IInteractive? _interactive;

        public InteractiveLayer(IInteractive interactive)
        {
            _interactive = interactive;

            _interactive.InvalidateLayer += (s, e) => DataHasChanged();

            IsMapInfoLayer = true;
        }

        public override IEnumerable<IFeature> GetFeatures(MRect box, double resolution)
        {
            if (_interactive is IDecorator decorator)
            {
                var feature = decorator.FeatureSource;

                if (feature == null)
                {
                    yield break;
                }

                if (box.Intersects(feature.Extent) == true)
                {
                    foreach (var point in decorator.GetActiveVertices())
                    {
                        yield return new GeometryFeature { Geometry = point.ToPoint() };
                    }
                }
            }
            else if (_interactive is IDesigner designer)
            {
                var feature = designer.Feature;

                yield return feature;

                if (designer.ExtraFeatures.Count != 0)
                {
                    foreach (var item in designer.ExtraFeatures)
                    {
                        yield return item;
                    }
                }

                foreach (var point in designer.GetActiveVertices())
                {
                    yield return new GeometryFeature { Geometry = point.ToPoint() };
                }
            }
        }

        public override void RefreshData(FetchInfo fetchInfo)
        {
            OnDataChanged(new DataChangedEventArgs());
        }
    }
}
