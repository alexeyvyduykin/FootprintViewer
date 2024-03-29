﻿using FootprintViewer.Data.Models;
using FootprintViewer.Layers;
using FootprintViewer.Models;
using FootprintViewer.Styles;
using FootprintViewer.UI.ViewModels.Tips;
using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Interactivity.Extensions;
using Mapsui.Interactivity.UI;
using Mapsui.Layers;
using Nito.Disposables.Internals;
using ReactiveUI;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.UI.ViewModels;

public partial class MainViewModel
{
    public void ResetCommand()
    {
        Interactive?.Cancel();
        Interactive = null;

        _selector = null;

        State = States.Default;

        HideTip();
    }

    public void RectangleCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<RectangleDesigner>()
            .AttachTo(Map)
            .Build();

        designer.HoverCreating
            .Select(s => FormatHelper.ToArea(s.Area()))
            .Subscribe(s => ShowTip(CustomTipViewModel.HoverCreating(TipTarget.Rectangle, s)));

        designer.EndCreating
            .Subscribe(s =>
            {
                HideTip();

                InfoPanel.Show(CreateAOIPanel(s));

                _mapService.AOI.Update(s, FeatureType.AOIRectangle);

                _mapService.State.Reset();
            });

        ShowTip(CustomTipViewModel.Init(TipTarget.Rectangle));

        Interactive = designer;

        State = States.Drawing;
    }

    public void PolygonCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<PolygonDesigner>()
            .AttachTo(Map)
            .Build();

        designer.BeginCreating
            .Subscribe(s => ShowTip(CustomTipViewModel.BeginCreating(TipTarget.Polygon)));

        designer.Creating
            .Select(s => s.Area())
            .Where(s => s != 0.0)
            .Select(s => FormatHelper.ToArea(s))
            .Subscribe(s => ShowTip(CustomTipViewModel.Creating(TipTarget.Polygon, s)));

        designer.EndCreating
            .Subscribe(s =>
            {
                HideTip();

                InfoPanel.Show(CreateAOIPanel(s));

                _mapService.AOI.Update(s, FeatureType.AOIPolygon);

                _mapService.State.Reset();
            });

        ShowTip(CustomTipViewModel.Init(TipTarget.Polygon));

        Interactive = designer;

        State = States.Drawing;
    }

    public void CircleCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<CircleDesigner>()
            .AttachTo(Map)
            .Build();

        designer.HoverCreating
            .Select(s => FormatHelper.ToArea(s.Area()))
            .Subscribe(s => ShowTip(CustomTipViewModel.HoverCreating(TipTarget.Circle, s)));

        designer.EndCreating
            .Subscribe(s =>
            {
                HideTip();

                InfoPanel.Show(CreateAOIPanel(s));

                _mapService.AOI.Update(s, FeatureType.AOICircle);

                _mapService.State.Reset();
            });

        ShowTip(CustomTipViewModel.Init(TipTarget.Circle));

        Interactive = designer;

        State = States.Drawing;
    }

    public void RouteCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<RouteDesigner>()
            .AttachTo(Map)
            .Build();

        var layer = Map.GetLayer<EditLayer>(LayerType.Edit);

        designer.BeginCreating
            .Select(s => FormatHelper.ToDistance(s.Distance()))
            .Subscribe(s => ShowTip(CustomTipViewModel.BeginCreating(TipTarget.Route, s)));

        designer.Creating
            .Subscribe(s => InfoPanel.Show(CreateRoutePanel(s)));

        designer.HoverCreating
            .Select(s => FormatHelper.ToDistance(s.Distance()))
            .Subscribe(s => ShowTip(CustomTipViewModel.HoverCreating(TipTarget.Route, s)));

        designer.EndCreating
            .Subscribe(s =>
            {
                layer?.AddRoute(new InteractiveRoute(s), FeatureType.Route.ToString());

                HideTip();

                _mapService.State.Reset();
            });

        layer?.ClearRoute();

        InfoPanel.CloseAll("Route");

        ShowTip(CustomTipViewModel.Init(TipTarget.Route));

        Interactive = designer;

        State = States.Drawing;
    }

    public void SelectCommand()
    {
        var types = new[] { LayerType.Footprint, LayerType.GroundTarget, LayerType.User };

        var availableLayers = types
            .Select(s => Map.GetLayer(s))
            .WhereNotNull()
            .ToArray();

        _selector?.Cancel();

        _selector = new InteractiveBuilder()
            .SelectSelector<Selector>()
            .AttachTo(Map)
            .AvailableFor(availableLayers)
            .Build();

        _selector.Select
            .Subscribe(async s =>
            {
                _mapService.SelectFeature(s.Layer, s.Feature);
                await OpenInfoPanel(s.Layer, s.Feature);
            });

        _selector.Unselect
            .Subscribe(s =>
            {
                _mapService.UnselectFeature(s.Layer);
                CloseInfoPanel(s.Layer);
            });

        _selector.HoverBegin
            .Subscribe(s => _mapService.EnterFeature(s.Layer, s.Feature));

        _selector.HoverEnd
            .Subscribe(s => _mapService.LeaveFeature(s.Layer));

        Interactive = _selector;

        State = States.Selecting;
    }

    public void ScaleCommand()
    {
        var userLayer = Map.GetLayer<ILayer>(LayerType.User);

        if (userLayer == null)
        {
            return;
        }

        _selector?.Cancel();

        _selector = new InteractiveBuilder()
            .SelectDecorator<ScaleDecorator>()
            .AttachTo(Map)
            .WithSelector<Selector>()
            .AvailableFor(userLayer)
            .Build();

        ((IDecoratorSelector)_selector).DecoratorSelecting.Subscribe(s =>
        {
            Interactive = s;
            State = States.Editing;
        });

        ((IDecoratorSelector)_selector).DecoratorUnselecting.Subscribe(s =>
        {
            Interactive = _selector;

            State = States.Selecting;

            EditFeature(s);
        });

        Interactive = _selector;

        State = States.Selecting;
    }

    public void TranslateCommand()
    {
        var userLayer = Map.GetLayer<ILayer>(LayerType.User);

        if (userLayer == null)
        {
            return;
        }

        _selector?.Cancel();

        _selector = new InteractiveBuilder()
            .SelectDecorator<TranslateDecorator>()
            .AttachTo(Map)
            .WithSelector<Selector>()
            .AvailableFor(userLayer)
            .Build();

        ((IDecoratorSelector)_selector).DecoratorSelecting.Subscribe(s =>
        {
            Interactive = s;
            State = States.Editing;
        });

        ((IDecoratorSelector)_selector).DecoratorUnselecting.Subscribe(s =>
        {
            Interactive = _selector;

            State = States.Selecting;

            EditFeature(s);
        });

        Interactive = _selector;

        State = States.Selecting;
    }

    public void RotateCommand()
    {
        var userLayer = Map.GetLayer<ILayer>(LayerType.User);

        if (userLayer == null)
        {
            return;
        }

        _selector?.Cancel();

        _selector = new InteractiveBuilder()
            .SelectDecorator<RotateDecorator>()
            .AttachTo(Map)
            .WithSelector<Selector>()
            .AvailableFor(userLayer)
            .Build();

        ((IDecoratorSelector)_selector).DecoratorSelecting.Subscribe(s =>
        {
            Interactive = s;
            State = States.Editing;
        });

        ((IDecoratorSelector)_selector).DecoratorUnselecting.Subscribe(s =>
        {
            Interactive = _selector;
            State = States.Selecting;
            EditFeature(s);
        });

        Interactive = _selector;
        State = States.Selecting;
    }

    public void EditCommand()
    {
        var userLayer = Map.GetLayer<ILayer>(LayerType.User);

        if (userLayer == null)
        {
            return;
        }

        _selector?.Cancel();

        _selector = new InteractiveBuilder()
            .SelectDecorator<EditDecorator>()
            .AttachTo(Map)
            .WithSelector<Selector>()
            .AvailableFor(userLayer)
            .Build();

        ((IDecoratorSelector)_selector).DecoratorSelecting.Subscribe(s =>
        {
            Interactive = s;
            State = States.Editing;
        });

        ((IDecoratorSelector)_selector).DecoratorUnselecting.Subscribe(s =>
        {
            Interactive = _selector;
            State = States.Selecting;
            EditFeature(s);
        });

        Interactive = _selector;
        State = States.Selecting;
    }

    public void DrawingPointCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<PointDesigner>()
            .AttachTo(Map)
            .Build();

        designer.EndCreating.Subscribe(s =>
        {
            AddUserGeometry(s, UserGeometryType.Point);

            HideTip();

            _mapService.State.Reset();
        });

        ShowTip(CustomTipViewModel.Init(TipTarget.Point));

        Interactive = designer;

        State = States.Drawing;
    }

    public void DrawingRectangleCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<RectangleDesigner>()
            .AttachTo(Map)
            .Build();

        designer.HoverCreating
            .Select(s => FormatHelper.ToArea(s.Area()))
            .Subscribe(s => ShowTip(CustomTipViewModel.HoverCreating(TipTarget.Rectangle, s)));

        designer.EndCreating
            .Subscribe(s =>
            {
                AddUserGeometry(s, UserGeometryType.Rectangle);

                HideTip();

                _mapService.State.Reset();
            });

        ShowTip(CustomTipViewModel.Init(TipTarget.Rectangle));

        Interactive = designer;

        State = States.Drawing;
    }

    public void DrawingCircleCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<CircleDesigner>()
            .AttachTo(Map)
            .Build();

        designer.HoverCreating
            .Select(s => FormatHelper.ToArea(s.Area()))
            .Subscribe(s => ShowTip(CustomTipViewModel.HoverCreating(TipTarget.Circle, s)));

        designer.EndCreating
            .Subscribe(s =>
            {
                AddUserGeometry(s, UserGeometryType.Circle);

                HideTip();

                _mapService.State.Reset();
            });

        ShowTip(CustomTipViewModel.Init(TipTarget.Circle));

        Interactive = designer;

        State = States.Drawing;
    }

    public void DrawingRouteCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<RouteDesigner>()
            .AttachTo(Map)
            .Build();

        designer.HoverCreating
            .Select(s => FormatHelper.ToDistance(s.Distance()))
            .Subscribe(s => ShowTip(CustomTipViewModel.HoverCreating(TipTarget.Route, s)));

        designer.EndCreating
            .Subscribe(s =>
            {
                HideTip();

                _mapService.State.Reset();
            });

        ShowTip(CustomTipViewModel.Init(TipTarget.Route));

        Interactive = designer;

        State = States.Drawing;
    }

    public void DrawingPolygonCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<PolygonDesigner>()
            .AttachTo(Map)
            .Build();

        designer.BeginCreating
            .Subscribe(s => ShowTip(CustomTipViewModel.BeginCreating(TipTarget.Polygon)));

        designer.Creating
            .Select(s => s.Area())
            .Where(s => s != 0.0)
            .Select(s => FormatHelper.ToArea(s))
            .Subscribe(s => ShowTip(CustomTipViewModel.Creating(TipTarget.Polygon, s)));

        designer.EndCreating
            .Subscribe(s =>
            {
                AddUserGeometry(s, UserGeometryType.Polygon);

                HideTip();

                _mapService.State.Reset();
            });

        ShowTip(CustomTipViewModel.Init(TipTarget.Polygon));

        Interactive = designer;

        State = States.Drawing;
    }
}
