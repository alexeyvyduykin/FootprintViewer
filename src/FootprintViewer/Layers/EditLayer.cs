using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer
{
    public class EditLayer : BaseLayer
    {   
        private List<AddInfo> _aoiInfos = new List<AddInfo>();
        private AddInfo _routeInfo = new AddInfo();

        private WritableLayer _layer = new WritableLayer();

        public EditLayer() : base()
        {

        }

        public override BoundingBox Envelope => _layer.Envelope;

        public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            return _layer.GetFeaturesInView(box, resolution);
        }

        public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType)
        {
            _layer.RefreshData(extent, resolution, changeType);
        }

        private AddInfo GetCurrentInfo()
        {
            return _aoiInfos.LastOrDefault();
        }

        public void AddAOI(AddInfo addInfo)
        {
            if (addInfo.Feature != null)
            {
                _aoiInfos.Add(addInfo);

                _layer.Add(addInfo.Feature);

                if (addInfo.HelpFeatures != null && addInfo.HelpFeatures.Count > 0)
                {
                    _layer.AddRange(addInfo.HelpFeatures);
                }
            }
        }

        public void ResetAOI()
        {
            foreach (var item in _aoiInfos)
            {
                _layer.TryRemove(item.Feature);
            }

            _aoiInfos.Clear();
        }

        public void ClearAOIHelpers()
        {
            var aoiInfo = GetCurrentInfo();
            
            if (aoiInfo.HelpFeatures != null && aoiInfo.HelpFeatures.Count > 0)
            {
                foreach (var item in aoiInfo.HelpFeatures)
                {
                    _layer.TryRemove(item);
                }

                aoiInfo.HelpFeatures = null;
            }
        }

        public void ClearRoute()
        {
            ClearRouteHelpers();

            if (_routeInfo.Feature != null)
            {
                _layer.TryRemove(_routeInfo.Feature);
                _routeInfo.Feature = null;
            }

            DataHasChanged();
        }

        public void ClearRouteHelpers()
        {
            if (_routeInfo.HelpFeatures != null && _routeInfo.HelpFeatures.Count > 0)
            {
                foreach (var item in _routeInfo.HelpFeatures)
                {
                    _layer.TryRemove(item);
                }

                _routeInfo.HelpFeatures = null;
            }
        }

        public void AddRoute(AddInfo addInfo)
        {
            ClearRoute();

            if (addInfo.Feature != null)
            {
                _routeInfo = addInfo;

                _layer.Add(addInfo.Feature);

                if (addInfo.HelpFeatures != null && addInfo.HelpFeatures.Count > 0)
                {
                    _layer.AddRange(addInfo.HelpFeatures);
                }
            }
        }


    }
}
