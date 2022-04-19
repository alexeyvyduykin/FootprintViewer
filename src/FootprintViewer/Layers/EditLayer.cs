using Mapsui;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;
using System.Collections.Generic;

namespace FootprintViewer.Layers
{
    public class EditLayer : BaseLayer
    {
        private readonly List<IFeature> _aoiInfos = new List<IFeature>();
        private IFeature? _routeInfo = null;

        private readonly WritableLayer _layer = new WritableLayer();

        public EditLayer() : base()
        {
            IsMapInfoLayer = true;
        }

        public List<MPoint> GetVertices()
        {
            var features = _layer.GetFeatures();

            List<MPoint> list = new List<MPoint>();

            foreach (var f in features)
            {
                if (f is InteractiveFeature interactiveFeature)
                {
                    list.AddRange(interactiveFeature.EditVertices());
                }
            }

            return list;
        }

        //public override BoundingBox Envelope => _layer.Envelope;

        public override IEnumerable<IFeature> GetFeatures(MRect box, double resolution)
        {
            return _layer.GetFeatures(box, resolution);
        }

       // public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType)
        public override void RefreshData(FetchInfo fetchInfo)
        {
            //_layer.RefreshData(extent, resolution, changeType);
            _layer.RefreshData(fetchInfo);
        }

        public void AddAOI(InteractiveFeature feature, string name)
        {
            feature["Name"] = name;

            ResetAOI();

            _aoiInfos.Add(feature);

            _layer.Add(feature);

            DataHasChanged();
        }

        public void ResetAOI()
        {
            foreach (var item in _aoiInfos)
            {
                _layer.TryRemove(item);
            }

            _aoiInfos.Clear();
        }

        public void ClearRoute()
        {
            if (_routeInfo != null)
            {
                _layer.TryRemove(_routeInfo);
                _routeInfo = null;
            }

            DataHasChanged();
        }

        public void AddRoute(InteractiveFeature feature, string name)
        {
            feature["Name"] = name;

            ClearRoute();

            _routeInfo = feature;

            _layer.Add(feature);

            DataHasChanged();
        }
    }
}
