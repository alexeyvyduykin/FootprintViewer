﻿using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Styles;
using System.Collections.Generic;
using System.Linq;

namespace InteractiveGeometry
{
    public class InteractiveFactory
    {
        private readonly IStyle _defaultSelectorStyle = new VectorStyle()
        {
            Fill = new Brush(Color.Transparent),
            Outline = new Pen(Color.Red, 4),
            Line = new Pen(Color.Red, 4),
        };

        public IDesigner CreatePolygonDesigner(IMap map, WritableLayer source)
        {
            return CreateDesigner(map, source, new PolygonDesigner());
        }

        public IDesigner CreateRouteDesigner(IMap map, WritableLayer source)
        {
            return CreateDesigner(map, source, new RouteDesigner());
        }

        public IDesigner CreateCircleDesigner(IMap map, WritableLayer source)
        {
            return CreateDesigner(map, source, new CircleDesigner());
        }

        public IDesigner CreateRectangleDesigner(IMap map, WritableLayer source)
        {
            return CreateDesigner(map, source, new RectangleDesigner());
        }

        private IDesigner CreateDesigner(IMap map, WritableLayer source, IDesigner designer)
        {
            RemoveInteractiveLayer(map);

            var interactiveLayer = new InteractiveLayer(designer) { Name = nameof(InteractiveLayer) };

            designer.BeginCreating += (s, e) =>
            {
                map.Layers.Add(interactiveLayer);
            };

            designer.EndCreating += (s, e) =>
            {
                source.Add(designer.Feature.Copy());

                map.Layers.Remove(interactiveLayer);
            };

            return designer;
        }

        public ISelectDecorator CreateSelectDecorator(Map map, ILayer layer, IStyle? style = null)
        {
            var selectDecorator = new SelectDecorator(map, layer);

            ICollection<IStyle>? saveStyles = null;
            BaseFeature? saveFeature = null;

            selectDecorator.Select += (s, e) =>
            {
                if (s is ISelectDecorator decorator)
                {
                    if (saveFeature != null)
                    {
                        saveFeature.Styles = saveStyles!;
                    }

                    saveFeature = decorator.SelectFeature!;
                    saveStyles = decorator.SelectFeature!.Styles;

                    decorator.SelectFeature.Styles = new StyleCollection() { style ?? _defaultSelectorStyle };
                }
            };

            selectDecorator.Unselect += (s, e) =>
            {
                if (s is ISelectDecorator decorator)
                {
                    if (saveFeature != null)
                    {
                        saveFeature.Styles = saveStyles!;

                        saveFeature = null;
                        saveStyles = null;
                    }
                }
            };

            return selectDecorator;
        }

        private void RemoveInteractiveLayer(IMap map)
        {
            var interactiveLayer = map.Layers.FindLayer(nameof(InteractiveLayer)).FirstOrDefault();

            if (interactiveLayer != null)
            {
                map.Layers.Remove(interactiveLayer);
            }
        }
    }
}
