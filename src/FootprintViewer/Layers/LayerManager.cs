using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FootprintViewer.Layers
{
    public class LayerManager
    {
        private readonly ILayer _layer;
        private IFeature? _lastSelected;
        private readonly Func<IEnumerable<IFeature>> _getFeatures;

        private LayerManager(ILayer layer, Func<IEnumerable<IFeature>> getFeatures)
        {
            _layer = layer;
            _getFeatures = getFeatures;
        }

        public IFeature? GetFeature(string name)
        {
            return _getFeatures.Invoke().Where(s => name.Equals((string)s["Name"]!)).FirstOrDefault();
        }

        public void SelectFeature(string name)
        {
            var feature = GetFeature(name);

            if (feature != null)
            {
                if (_lastSelected != null)
                {
                    _lastSelected[InteractiveFields.Select] = false;
                }

                feature[InteractiveFields.Select] = true;

                _lastSelected = feature;

                _layer.DataHasChanged();
            }
        }

        public void ShowHighlight(string name)
        {
            var feature = GetFeature(name);

            if (feature != null)
            {
                feature["Highlight"] = true;

                _layer.DataHasChanged();
            }
        }

        public void HideHighlight()
        {
            foreach (var item in _getFeatures.Invoke())
            {
                item["Highlight"] = false;
            }

            _layer.DataHasChanged();
        }
    }

    public static class LayerManagerExtensions
    {
        public static LayerManager? BuildManager(this ILayer layer, Func<IEnumerable<IFeature>> getFeatures)
        {
            Type type = typeof(LayerManager);
            return (LayerManager?)Activator.CreateInstance(
                type,
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] { layer, getFeatures },
                null,
                null);
        }
    }
}
