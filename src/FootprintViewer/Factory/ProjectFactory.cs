﻿using FootprintViewer.Data;
using FootprintViewer.Layers;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using Splat;
using System;
using System.Linq;

namespace FootprintViewer
{
    public class ProjectFactory
    {
        private readonly Color EditModeColor = new Color(124, 22, 111, 180);

        private readonly SymbolStyle SelectedStyle = new SymbolStyle
        {
            Fill = null,
            Outline = new Pen(Color.Red, 3),
            Line = new Pen(Color.Red, 3)
        };

        private readonly SymbolStyle DisableStyle = new SymbolStyle { Enabled = false };

        public Map CreateMap(IReadonlyDependencyResolver dependencyResolver)
        {
            var satelliteProvider = dependencyResolver.GetExistingService<SatelliteProvider>();       
            var groundTargetProvider = dependencyResolver.GetExistingService<GroundTargetProvider>();       
            var footprintProvider = dependencyResolver.GetExistingService<FootprintProvider>();       
            var mapProvider = dependencyResolver.GetExistingService<MapProvider>();
            var customProvider = dependencyResolver.GetExistingService<CustomProvider>();

            var map = new Map()
            {
                CRS = "EPSG:3857",
                Transformation = new MinimalTransformation(),
            };

            map.Layers.Add(new Layer() { Name = nameof(LayerType.WorldMap) });                 // WorldMap
            map.Layers.Add(new WritableLayer { Name = nameof(LayerType.FootprintImage) });     // FootprintImage

            map.Layers.Add(new TargetLayer(new TargetLayerProvider(groundTargetProvider)));    // GroundTarget
            map.Layers.Add(new SensorLayer(new SensorLayerProvider(satelliteProvider)));       // Sensor
            map.Layers.Add(new TrackLayer(new TrackLayerProvider(satelliteProvider)));         // Track
            map.Layers.Add(new FootprintLayer(new FootprintLayerProvider(footprintProvider))); // Footprint

            map.Layers.Add(CreateFootprintBorderLayer());                                      // FootprintImageBorder

            var editLayer = CreateEmptyEditLayer();
            map.Layers.Add(editLayer); // Edit
            map.Layers.Add(new VertexOnlyLayer(editLayer) { Name = nameof(LayerType.Vertex) }); // Vertex

            //map.Home = (n) => n.NavigateTo(editLayer.Envelope.Grow(editLayer.Envelope.Width * 0.2));

            var userLayer = CreateLayer(customProvider);
            userLayer.Name = "FeatureLayer";         
            map.Layers.Add(userLayer);

            map.SetWorldMapLayer(mapProvider.GetMapResources().FirstOrDefault());

            return map;
        }

        private static ILayer CreateLayer(IProvider provider)
        {        
            Color backgroundColor = new Color(20, 120, 120, 40);
            Color lineColor = new Color(20, 120, 120);
            Color outlineColor = new Color(20, 20, 20);
        
            var polygonLayer = new Layer
            {
                DataSource = provider,
                IsMapInfoLayer = true,
                Style = new VectorStyle
                {
                    Fill = new Brush(new Color(backgroundColor)),
                    Line = new Pen(lineColor, 3),
                    Outline = new Pen(outlineColor, 3)
                },
            };

            return polygonLayer;
        }

        public InfoPanel CreateInfoPanel()
        {
            //InfoPanelItem routeItem = new InfoPanelItem()
            //{
            //    Title = "Route",
            //    Text = "Description",
            //    CommandTitle = "X",
            //};

            //InfoPanelItem aoiItem = new InfoPanelItem()
            //{
            //    Title = "AOI",
            //    Text = "Description",
            //    CommandTitle = "X",
            //};

            var infoPanel = new InfoPanel();

            //infoPanel.Items = new ObservableCollection<InfoPanelItem>(new[] { routeItem, aoiItem });

            return infoPanel;
        }


        public readonly Random random = new System.Random();

        private EditLayer CreateEmptyEditLayer()
        {
            var editLayer = new EditLayer
            {
                Name = nameof(LayerType.Edit),
                Style = CreateSelectedStyle(),
                IsMapInfoLayer = true
            };

            return editLayer;
        }

        public void Scale(IGeometry geometry, double scale, Point center)
        {
            foreach (var vertex in geometry.AllVertices())
            {
                Scale(vertex, scale, center);
            }
        }

        private void Scale(Point vertex, double scale, Point center)
        {
            vertex.X = center.X + (vertex.X - center.X) * scale;
            vertex.Y = center.Y + (vertex.Y - center.Y) * scale;
        }

        private WritableLayer CreateFootprintBorderLayer()
        {
            Color color = new Color() { R = 76, G = 185, B = 247, A = 255 };

            var layer = new WritableLayer
            {
                Name = nameof(LayerType.FootprintImageBorder),
                Style = new VectorStyle
                {
                    Fill = new Brush(Color.Opacity(color, 0.25f)),
                    Line = new Pen(color, 1.0),
                    Outline = new Pen(color, 1.0),
                    Enabled = true
                }
            };

            return layer;
        }

        private IStyle CreateSelectedStyle()
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

        private IStyle CreateEditLayerBasicStyle()
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
