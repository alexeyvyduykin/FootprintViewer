using InteractivitySample.Layers;
using Mapsui;
using Mapsui.Geometries.WellKnownText;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace InteractivitySample.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private static readonly Color PolygonBackgroundColor = new Color(20, 120, 120, 40);
        private static readonly Color PolygonLineColor = new Color(20, 120, 120, 255);
        private static readonly Color PolygonOutlineColor = new Color(20, 20, 20, 255);

        public MainViewModel()
        {
            Map = CreateMap();
        }

        public static Map CreateMap()
        {
            var map = new Map()
            {
                CRS = "EPSG:3857",
                // Transformation = new MinimalTransformation(),
            };

            var userLayer = CreatePolygonLayer();

            map.Layers.Add(new Layer()); // BackgroundLayer
            map.Layers.Add(userLayer);
            map.Layers.Add(new VertexOnlyLayer(userLayer));

            return map;
        }

        private static WritableLayer CreatePolygonLayer()
        {
            var polygonLayer = new WritableLayer
            {
                Name = "PolygonLayer",
                Style = CreatePolygonStyle()
            };

            var wkt = "POLYGON ((1261416.17275404 5360656.05714234, 1261367.50386493 5360614.2556425, 1261353.47050427 5360599.62511755, 1261338.83997932 5360576.03712836, 1261337.34706862 5360570.6626498, 1261375.8641649 5360511.2448036, 1261383.92588273 5360483.17808227, 1261391.98760055 5360485.56673941, 1261393.48051126 5360480.490843, 1261411.99260405 5360487.6568144, 1261430.50469684 5360496.9128608, 1261450.21111819 5360507.06465361, 1261472.00761454 5360525.5767464, 1261488.13105019 5360544.98458561, 1261488.1310502 5360545.28316775, 1261481.26366093 5360549.76189988, 1261489.6239609 5360560.21227484, 1261495.59560374 5360555.13637843, 1261512.91336796 5360573.05130694, 1261535.00844645 5360598.43078898, 1261540.08434286 5360619.03295677, 1261535.90419287 5360621.12303176, 1261526.64814648 5360623.21310675, 1261489.32537876 5360644.41243881, 1261458.27283602 5360661.73020303, 1261438.26783253 5360662.02878517, 1261427.22029328 5360660.23729232, 1261416.17275404 5360656.05714234))";
            var polygon = GeometryFromWKT.Parse(wkt);
            IFeature feature = new Feature { Geometry = polygon };
            polygonLayer.Add(feature);

            return polygonLayer;
        }

        private static IStyle CreatePolygonStyle()
        {
            return new VectorStyle
            {
                Fill = new Brush(new Color(PolygonBackgroundColor)),
                Line = new Pen(PolygonLineColor, 3),
                Outline = new Pen(PolygonOutlineColor, 3)
            };
        }

        [Reactive]
        public Map Map { get; set; }
    }
}
