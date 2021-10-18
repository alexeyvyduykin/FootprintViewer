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
    public class EditLayer : WritableLayer
    {   
        private List<AddInfo> _aoiInfos = new List<AddInfo>();
        private AddInfo _routeInfo = new AddInfo();

        public EditLayer() : base()
        {

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

                Add(addInfo.Feature);

                if (addInfo.HelpFeatures != null && addInfo.HelpFeatures.Count > 0)
                {
                    AddRange(addInfo.HelpFeatures);
                }
            }
        }

        public void ResetAOI()
        {
            foreach (var item in _aoiInfos)
            {
                TryRemove(item.Feature);
            }
        }

        public void ClearAOIHelpers()
        {
            var aoiInfo = GetCurrentInfo();
            
            if (aoiInfo.HelpFeatures != null && aoiInfo.HelpFeatures.Count > 0)
            {
                foreach (var item in aoiInfo.HelpFeatures)
                {
                    TryRemove(item);
                }

                aoiInfo.HelpFeatures = null;
            }
        }

        public void ClearRoute()
        {
            ClearRouteHelpers();

            if (_routeInfo.Feature != null)
            {                    
                TryRemove(_routeInfo.Feature);
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
                    TryRemove(item);
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

                Add(addInfo.Feature);

                if (addInfo.HelpFeatures != null && addInfo.HelpFeatures.Count > 0)
                {
                    AddRange(addInfo.HelpFeatures);
                }
            }
        }
    }
}
