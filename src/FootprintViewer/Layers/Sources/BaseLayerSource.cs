using Mapsui;
using Mapsui.Layers;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.Layers
{
    public interface ILayerSource : ILayer
    {
        IFeature? GetFeature(string name);

        void SelectFeature(string name);

        void ShowHighlight(string name);

        void HideHighlight();
    }

    public abstract class BaseLayerSource<TNative> : WritableLayer, ILayerSource
    {
        private IFeature? _lastSelected;

        public IFeature? GetFeature(string name)
        {
            return GetFeatures().Where(s => s.Fields.Contains("Name") && s["Name"]!.Equals(name)).FirstOrDefault();
        }

        public void SelectFeature(string name)
        {
            var feature = GetFeatures().Where(s => name.Equals((string)s["Name"]!)).First();

            if (_lastSelected != null)
            {
                _lastSelected["State"] = "Unselected";
            }

            feature["State"] = "Selected";

            _lastSelected = feature;

            DataHasChanged();
        }

        public void ShowHighlight(string name)
        {
            var feature = GetFeatures().Where(s => name.Equals((string)s["Name"]!)).First();

            feature["Highlight"] = true;

            DataHasChanged();
        }

        public void HideHighlight()
        {
            foreach (var item in GetFeatures())
            {
                item["Highlight"] = false;
            }

            DataHasChanged();
        }
    }
}
