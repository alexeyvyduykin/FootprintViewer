using BruTile;
using BruTile.FileSystem;
using BruTile.MbTiles;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Geometries.WellKnownText;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using Mapsui.UI;
using SQLite;
using System;

namespace FootprintViewer
{



    public static class SampleBuilder
    {
        private static readonly Color EditModeColor = new Color(124, 22, 111, 180);

        private static readonly SymbolStyle SelectedStyle = new SymbolStyle
        {
            Fill = null,
            Outline = new Pen(Color.Red, 3),
            Line = new Pen(Color.Red, 3)
        };

        private static readonly SymbolStyle DisableStyle = new SymbolStyle { Enabled = false };

        public static Map CreateMap()
        {
            var backgroundLayer = CreateBackgroundLayer();

            var map = new Map()
            {
                CRS = "EPSG:3857",
                Transformation = new MinimalTransformation(),
                Limiter = new ViewportLimiterKeepWithin { PanLimits = backgroundLayer.Envelope/*limiter*/ }
            };

            //map.Layers.Add(OpenStreetMap.CreateTileLayer());
            //map.Layers.Add(CreateOpenStreetMapLayer());

            map.Layers.Add(backgroundLayer); // BackgroundLayer
            map.Layers.Add(CreateEmptyFootprintLayer()); // FootprintLayer
            map.Layers.Add(CreateFootprintBorderLayer()); // FootprintBorderLayer

            var editLayer = CreateEditLayer();
            map.Layers.Add(editLayer);
            map.Layers.Add(new VertexOnlyLayer(editLayer) { Name = nameof(LayerType.VertexLayer) });

            return map;
        }

        public static readonly Random random = new System.Random();

        private static ILayer CreateOpenStreetMapLayer()
        {
            string UserAgent = "FootprintViewer 1.0";

            //string UserAgent = string.Format("Mozilla/5.0 (Windows NT {1}.0; {2}rv:{0}.0) Gecko/20100101 Firefox/{0}.0",
            //random.Next(DateTime.Today.Year - 1969 - 5, DateTime.Today.Year - 1969),
            //random.Next(0, 10) % 2 == 0 ? 10 : 6,
            //random.Next(0, 10) % 2 == 1 ? string.Empty : "WOW64; ");

            //var httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);

            //var osmAttribution = new Attribution("© OpenStreetMap contributors", "https://www.openstreetmap.org/copyright");
            //var osmSource = new HttpClientTileSource(httpClient, new GlobalSphericalMercator(), "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", new[] { "a", "b", "c" }, name: "OpenStreetMap", attribution: osmAttribution);
            //var osmLayer = new TileLayer(osmSource) { Name = "OpenStreetMap" };



            var attribution = new BruTile.Attribution("© OpenStreetMap contributors",
    "http://www.openstreetmap.org/copyright");


            var tileSource = new HttpTileSource(new GlobalSphericalMercator(),
                "https://cartodb-basemaps-{s}.global.ssl.fastly.net/light_all/{z}/{x}/{y}.png",
                new[] { "a", "b", "c" }, name: "OpenStreetMap",
                attribution: attribution, userAgent: "FootprintViewer");


            var tileLayer = new TileLayer(tileSource) { Name = "Carto Light" };


            return tileLayer;
        }

        private static EditLayer CreateEditLayer()
        {
            var editLayer = new EditLayer
            {
                Name = nameof(LayerType.EditLayer),
                Style = CreateSelectedStyle(),
                IsMapInfoLayer = true
            };

            var wkt = "POLYGON ((1261416.17275404 5360656.05714234, 1261367.50386493 5360614.2556425, 1261353.47050427 5360599.62511755, 1261338.83997932 5360576.03712836, 1261337.34706862 5360570.6626498, 1261375.8641649 5360511.2448036, 1261383.92588273 5360483.17808227, 1261391.98760055 5360485.56673941, 1261393.48051126 5360480.490843, 1261411.99260405 5360487.6568144, 1261430.50469684 5360496.9128608, 1261450.21111819 5360507.06465361, 1261472.00761454 5360525.5767464, 1261488.13105019 5360544.98458561, 1261488.1310502 5360545.28316775, 1261481.26366093 5360549.76189988, 1261489.6239609 5360560.21227484, 1261495.59560374 5360555.13637843, 1261512.91336796 5360573.05130694, 1261535.00844645 5360598.43078898, 1261540.08434286 5360619.03295677, 1261535.90419287 5360621.12303176, 1261526.64814648 5360623.21310675, 1261489.32537876 5360644.41243881, 1261458.27283602 5360661.73020303, 1261438.26783253 5360662.02878517, 1261427.22029328 5360660.23729232, 1261416.17275404 5360656.05714234))";
            var polygon = GeometryFromWKT.Parse(wkt);

            Scale(polygon, 2500.0, polygon.BoundingBox.Centroid);

            var feature = new Feature { Geometry = polygon };

            var interactiveFeature = new InteractivePolygon(feature);

            AddInfo addInfo = new AddInfo() { Feature = interactiveFeature };

            editLayer.AddAOI(addInfo);

            return editLayer;
        }

        public static void Scale(IGeometry geometry, double scale, Mapsui.Geometries.Point center)
        {
            foreach (var vertex in geometry.AllVertices())
            {
                Scale(vertex, scale, center);
            }
        }

        private static void Scale(Mapsui.Geometries.Point vertex, double scale, Mapsui.Geometries.Point center)
        {
            vertex.X = center.X + (vertex.X - center.X) * scale;
            vertex.Y = center.Y + (vertex.Y - center.Y) * scale;
        }

        private static ILayer CreateBackgroundLayer()
        {
            string path = @"C:/Users/User/AlexeyVyduykin/Resources/WorldMap/world.mbtiles";

            var mbTilesTileSource = new MbTilesTileSource(new SQLiteConnectionString(path, true));

            var area = mbTilesTileSource.Schema.Extent.ToBoundingBox();
            var delta = area.Width / 2.0;

            //double minX, double minY, double maxX, double maxY
            var limiter = new BoundingBox(area.Left - delta, area.Bottom, area.Right + delta, area.Top);

            var layer = new TileLayer(mbTilesTileSource)
            {
                Name = nameof(LayerType.BackgroundLayer)
            };

            return layer;
        }

        private static ITileSource CreateEarthMapTileSource()
        {
            var fileTileProvider =
                new FileTileProvider(@"C:/Users/User/AlexeyVyduykin/Resources/WorldMap/EarthMapXYZ", "png", new TimeSpan(long.MaxValue));

            // BruTile.Predefined.GlobalMercator

            // var schema = new GlobalMercator("EPSG:4326", 0, 5);
            // schema.YAxis = YAxis.OSM;

            var schema = new GlobalSphericalMercator(YAxis.OSM, 0, 5);

            var tileSource = new TileSource(fileTileProvider, schema);

            return tileSource;
        }

        private static WritableLayer CreateEmptyFootprintLayer()
        {
            var layer = new WritableLayer
            {
                Name = nameof(LayerType.FootprintLayer),
            };

            return layer;
        }

        private static WritableLayer CreateFootprintBorderLayer()
        {
            var layer = new WritableLayer
            {
                Name = nameof(LayerType.FootprintBorderLayer),
                Style = new VectorStyle
                {
                    Fill = new Brush(Color.Opacity(Color.Blue, 0.3f)),
                    Line = new Pen() { Color = Color.Blue, Width = 1.0 },
                    Enabled = true
                }
            };

            return layer;
        }

        private static IStyle CreateSelectedStyle()
        {
            // To show the selected style a ThemeStyle is used which switches on and off the SelectedStyle
            // depending on a "Selected" attribute.
            return new ThemeStyle(f =>
            {
                if (f.Geometry is Mapsui.Geometries.Point)
                {
                    return null;
                }

                if (f.Fields != null)
                {
                    foreach (var item in f.Fields)
                    {
                        if (item.Equals("Name"))
                        {
                            return FeatureStyles.Get((string)f["Name"]);
                        }
                    }
                }

                if (f.Geometry is Polygon)
                {
                    return CreateEditLayerBasicStyle();
                }

                return null;
            });
        }

        private static IStyle CreateEditLayerBasicStyle()
        {
            var editStyle = new VectorStyle
            {
                Fill = new Brush(EditModeColor),
                Line = new Pen(EditModeColor, 3),
                Outline = new Pen(EditModeColor, 3)
            };
            return editStyle;
        }
    }
}
