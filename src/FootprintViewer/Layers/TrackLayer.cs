using FootprintViewer.ViewModels;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer.Layers
{
    public class TrackLayer : MemoryLayer
    {     
        private IStyle _style;
        private TrackProvider _provider;

        public TrackLayer(IProvider provider)
        {
            _provider = (TrackProvider)provider;           
            _style = CreateTrackStyle();
          
            Name = nameof(LayerType.Track);
            Style = _style;
            DataSource = provider;
            IsMapInfoLayer = false;
        }

        public void Update(SatelliteInfo info) 
        {
            _provider.Update(info);
            DataHasChanged();
        }

        private static IStyle CreateTrackStyle()
        {
            var stl = new VectorStyle
            {
                Fill = null,
                Line = new Pen(Color.Green, 1),
            };

            return stl;
        }
    }
}
