﻿using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InteractivitySample.Layers
{
    public class VertexOnlyLayer : BaseLayer
    {
        private readonly ILayer _source;

        private static readonly SymbolStyle PointStyle = new SymbolStyle
        {
            Fill = new Brush(Color.White),
            Outline = new Pen(Color.Black, 2 / 0.3),
            SymbolType = SymbolType.Ellipse,
            SymbolScale = 0.3,       
        };
       
        public override BoundingBox Envelope => _source.Envelope;

        public VertexOnlyLayer(ILayer source)
        {
            _source = source;
            _source.DataChanged += (sender, args) => OnDataChanged(args);
            Style = PointStyle;
        }

        public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            var features = _source.GetFeaturesInView(box, resolution).ToList();
            foreach (var feature in features)
            {
                if (feature.Geometry is Point || feature.Geometry is MultiPoint)
                {
                    continue; 
                }

                if (feature.Geometry != null)
                {
                    foreach (var vertices in feature.Geometry.MainVertices())
                    {
                        yield return new Feature { Geometry = vertices };
                    }
                }
            }
        }

        public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType)
        {
            OnDataChanged(new DataChangedEventArgs());
        }
    }
}