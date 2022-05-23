using Mapsui;
using Mapsui.Layers;
using System.Collections.Generic;

namespace FootprintViewer.Layers
{
    public interface IEditLayerSource : ILayer
    {
        void AddAOI(InteractiveFeature feature, string name);

        void ResetAOI();

        void AddRoute(InteractiveFeature feature, string name);

        void ClearRoute();
    }

    public class EditLayerSource : WritableLayer, IEditLayerSource
    {
        private readonly List<IFeature> _aoiInfos = new();
        private IFeature? _routeInfo = null;

        public EditLayerSource() : base()
        {

        }

        public void AddAOI(InteractiveFeature feature, string name)
        {
            feature["Name"] = name;

            ResetAOI();

            _aoiInfos.Add(feature);

            Add(feature);

            DataHasChanged();
        }

        public void ResetAOI()
        {
            foreach (var item in _aoiInfos)
            {
                TryRemove(item);
            }

            _aoiInfos.Clear();

            DataHasChanged();
        }

        public void ClearRoute()
        {
            if (_routeInfo != null)
            {
                TryRemove(_routeInfo);
                _routeInfo = null;
            }

            DataHasChanged();
        }

        public void AddRoute(InteractiveFeature feature, string name)
        {
            feature["Name"] = name;

            ClearRoute();

            _routeInfo = feature;

            Add(feature);

            DataHasChanged();
        }
    }
}
