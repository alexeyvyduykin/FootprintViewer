using FootprintViewer.ViewModels;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer.Layers
{
    public class SensorLayer : MemoryLayer
    {
        private IStyle _style;
        private SensorProvider _provider;

        public SensorLayer(IProvider provider)
        {
            _provider = (SensorProvider)provider;
            _style = CreateSensorStyle();

            Name = nameof(LayerType.Sensor);
            Style = _style;
            DataSource = provider;
            IsMapInfoLayer = false;
        }

        public void Update(SatelliteInfo info)
        {
            _provider.Update(info);
            DataHasChanged();
        }

        private static IStyle CreateSensorStyle()
        {
            var stl = new VectorStyle
            {
                Fill = new Brush(Color.Opacity(Color.Blue, 0.25f)),
                Line = null,
                Outline = null,
            };

            return stl;
        }
    }
}
