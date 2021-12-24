using InteractivitySample.Interactivity.Decorators;
using InteractivitySample.Interactivity.Designers;
using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using System.Collections.Generic;

namespace InteractivitySample.Interactivity
{
    public class InteractiveLayer : BaseLayer
    {
        private readonly ILayer _source;
        private readonly IInteractiveObject? _interactiveObject;
   
        public override BoundingBox Envelope => _source.Envelope;

        public InteractiveLayer(ILayer source, IInteractiveObject interactiveObject)
        {
            _source = source;
            _interactiveObject = interactiveObject;
            _source.DataChanged += (sender, args) => OnDataChanged(args);

            _interactiveObject.InvalidateLayer += (s, e) => DataHasChanged();
        }

        public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            if (_interactiveObject is IDecorator decorator)
            {
                var feature = decorator.FeatureSource;

                if (feature == null)
                {
                    yield break;
                }

                if (box.Intersects(feature.Geometry.BoundingBox) == true)
                {
                    foreach (var point in decorator.GetActiveVertices())
                    {
                        yield return new Feature { Geometry = point };
                    }
                }
            }
            else if (_interactiveObject is IDesigner designer)
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

            }
        }

        public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType)
        {
            OnDataChanged(new DataChangedEventArgs());
        }
    }
}
