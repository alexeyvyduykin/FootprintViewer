using Mapsui;
using Mapsui.Layers;

namespace FootprintViewer.Layers
{
    public class TargetLayer : Layer
    {
        private IFeature? _lastSelected;

        public void SelectFeature(string name)
        {
            if (DataSource is ITargetLayerSource source)
            {
                var feature = source.GetFeature(name);

                if (feature != null)
                {
                    if (_lastSelected != null)
                    {
                        _lastSelected["State"] = "Unselected";
                    }

                    feature["State"] = "Selected";

                    _lastSelected = feature;

                    DataHasChanged();
                }
            }
        }

        public void ShowHighlight(string name)
        {
            if (DataSource is ITargetLayerSource source)
            {
                var feature = source.GetFeature(name);

                if (feature != null)
                {
                    feature["Highlight"] = true;

                    DataHasChanged();
                }
            }
        }

        public void HideHighlight()
        {
            if (DataSource is ITargetLayerSource source)
            {
                foreach (var item in source.GetFeatures())
                {
                    item["Highlight"] = false;
                }

                DataHasChanged();
            }
        }
    }
}
