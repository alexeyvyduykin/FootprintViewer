using FootprintViewer.Interactivity.Decorators;
using FootprintViewer.Interactivity.Designers;
using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using System.Collections.Generic;

namespace FootprintViewer.Interactivity
{
    public class InteractiveLayer : BaseLayer
    {
        private readonly ILayer _source;
        private readonly IInteractiveObject? _interactiveObject;

        //public override BoundingBox Envelope => _source.Envelope;

        public InteractiveLayer(ILayer source, IInteractiveObject interactiveObject)
        {
            _source = source;
            _interactiveObject = interactiveObject;
            _source.DataChanged += (sender, args) => OnDataChanged(args);

            _interactiveObject.InvalidateLayer += (s, e) => DataHasChanged();

            IsMapInfoLayer = true;
        }

        public override IEnumerable<IFeature> GetFeatures(MRect box, double resolution)
        {
            if (_interactiveObject is IDecorator decorator)
            {
                var feature = decorator.FeatureSource;

                if (feature == null)
                {
                    yield break;
                }

                if (box.Intersects(feature.Extent /*Geometry.BoundingBox*/) == true)
                {
                    foreach (var point in decorator.GetActiveVertices())
                    {
                        yield return new GeometryFeature { Geometry = point.ToPoint() };
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

                foreach (var point in designer.GetActiveVertices())
                {
                    yield return new GeometryFeature { Geometry = point.ToPoint() };
                }
            }
        }
        public override void RefreshData(FetchInfo fetchInfo)
        //public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType)
        {
            OnDataChanged(new DataChangedEventArgs());
        }      
    }
}
