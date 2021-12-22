using InteractivitySample.Decorators;
using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using System.Collections.Generic;

namespace InteractivitySample.Layers
{
    public class InteractiveLayer : BaseLayer
    {
        private readonly ILayer _source;
        private readonly IDecorator _decorator;

        public override BoundingBox Envelope => _source.Envelope;

        public InteractiveLayer(ILayer source, IDecorator decorator)
        {
            _source = source;
            _decorator = decorator;
            _source.DataChanged += (sender, args) => OnDataChanged(args);
        }

        public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            var feature = _decorator.FeatureSource;

            if (feature == null)
            {
                yield break;
            }

            if (box.Intersects(feature.Geometry.BoundingBox) == true)
            {
                foreach (var point in _decorator.GetActiveVertices())
                {
                    yield return new Feature { Geometry = point };
                }
            }
        }

        public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType)
        {
            OnDataChanged(new DataChangedEventArgs());
        }
    }
}
