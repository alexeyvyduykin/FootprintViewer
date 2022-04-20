using Mapsui;
using Mapsui.Layers;
using System.Collections.Generic;

namespace FootprintViewer.Layers
{
    public class EditLayer : BaseLayer
    {
        private readonly List<IFeature> _aoiInfos = new();
        private IFeature? _routeInfo = null;
        private readonly WritableLayer _layer = new();

        public EditLayer() : base()
        {
            IsMapInfoLayer = true;
        }

        public List<MPoint> GetVertices()
        {
            var features = _layer.GetFeatures();

            var list = new List<MPoint>();

            foreach (var f in features)
            {
                if (f is InteractiveFeature interactiveFeature)
                {
                    list.AddRange(interactiveFeature.EditVertices());
                }
            }

            return list;
        }

        public override IEnumerable<IFeature> GetFeatures(MRect box, double resolution)
        {
            return _layer.GetFeatures(box, resolution);
        }

        public override void RefreshData(FetchInfo fetchInfo)
        {
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
