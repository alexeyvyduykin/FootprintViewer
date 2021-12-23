using InteractivitySample.Decorators;
using InteractivitySample.FeatureBuilders;
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
        private IDecorator? _decorator;
        private IFeatureBuilder? _builder;

        public override BoundingBox Envelope => _source.Envelope;

        public InteractiveLayer(ILayer source, IDecorator decorator)
        {
            _source = source;
            _decorator = decorator;
            _source.DataChanged += (sender, args) => OnDataChanged(args);
        }

        public InteractiveLayer(ILayer source, IFeatureBuilder builder)
        {
            _source = source;
            _builder = builder;
            _builder.InvalidateLayer += (s, e) => DataHasChanged();
            _source.DataChanged += (sender, args) => OnDataChanged(args);
        }

        public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            if (_decorator != null)
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
            else if (_builder != null)                    
            {
                var feature = _builder.Feature;

                yield return feature;

                if (_builder.ExtraFeatures.Count != 0)
                {
                    foreach (var item in _builder.ExtraFeatures)
                    {
                        yield return item;
                    }
                }

            }
        }

        public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType)
        {
            OnDataChanged(new DataChangedEventArgs());
        }
    }
}
