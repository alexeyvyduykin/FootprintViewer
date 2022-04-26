using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Nts;
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

        public ISelectScaleDecorator CreateSelectScaleDecorator(Map map, ILayer layer)
        {
            var scaleDecorator = new SelectScaleDecorator(map, layer);

            scaleDecorator.DecoratorChanged += (s, e) =>
            {
                if (s is SelectScaleDecorator decorator)
                {
                    if (decorator.Scale != null)
                    {
                        var interactiveLayer = new InteractiveLayer(decorator.Scale) { Name = nameof(InteractiveLayer) };

                        map.Layers.Add(interactiveLayer);
                    }
                    else
                    {
                        RemoveInteractiveLayer(map);
                    }
                }
            };

            return scaleDecorator;
        }

        public ISelectTranslateDecorator CreateSelectTranslateDecorator(Map map, ILayer layer)
        {
            var translateDecorator = new SelectTranslateDecorator(map, layer);

            translateDecorator.DecoratorChanged += (s, e) =>
            {
                if (s is SelectTranslateDecorator decorator)
                {
                    if (decorator.Translate != null)
                    {
                        var interactiveLayer = new InteractiveLayer(decorator.Translate) { Name = nameof(InteractiveLayer) };

                        map.Layers.Add(interactiveLayer);
                    }
                    else
                    {
                        RemoveInteractiveLayer(map);
                    }
                }
            };

            return translateDecorator;
        }

        public ISelectRotateDecorator CreateSelectRotateDecorator(Map map, ILayer layer)
        {
            var rotateDecorator = new SelectRotateDecorator(map, layer);

            rotateDecorator.DecoratorChanged += (s, e) =>
            {
                if (s is SelectRotateDecorator decorator)
                {
                    if (decorator.Rotate != null)
                    {
                        var interactiveLayer = new InteractiveLayer(decorator.Rotate) { Name = nameof(InteractiveLayer) };

                        map.Layers.Add(interactiveLayer);
                    }
                    else
                    {
                        RemoveInteractiveLayer(map);
                    }
                }
            };

            return rotateDecorator;
        }

        public ISelectEditDecorator CreateSelectEditDecorator(Map map, ILayer layer)
        {
            var editDecorator = new SelectEditDecorator(map, layer);

            editDecorator.DecoratorChanged += (s, e) =>
            {
                if (s is SelectEditDecorator decorator)
                {
                    if (decorator.Edit != null)
                    {
                        var interactiveLayer = new InteractiveLayer(decorator.Edit) { Name = nameof(InteractiveLayer) };

                        map.Layers.Add(interactiveLayer);
                    }
                    else
                    {
                        RemoveInteractiveLayer(map);
                    }
                }
            };

            return editDecorator;
        }

        public IDecorator CreateScaleDecorator(GeometryFeature feature)
        {
            var decorator = new ScaleDecorator(feature);

            return decorator;
        }

        public IDecorator CreateTranslateDecorator(GeometryFeature feature)
        {
            var decorator = new TranslateDecorator(feature);

            return decorator;
        }

        public IDecorator CreateRotateDecorator(GeometryFeature feature)
        {
            var decorator = new RotateDecorator(feature);

            return decorator;
        }

        public IDecorator CreateEditDecorator(GeometryFeature feature)
        {
            var decorator = new EditDecorator(feature);

            return decorator;
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
