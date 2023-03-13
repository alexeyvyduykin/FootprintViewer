using Stateless;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ToolBarSample.Models;

public class MapState
{
    private enum Trigger
    {
        Reset,
        Selectable,
        RectAOIDrawing,
        CircleAOIDrawing,
        PolygonAOIDrawing,
        RouteDrawing,
        Translating,
        Rotating,
        Scaling,
        Editing,
        PointDrawing,
        RectDrawing,
        CircleDrawing,
        PolygonDrawing
    }

    private States _state = States.Default;
    private readonly StateMachine<States, Trigger> _machine;
    private readonly Subject<Unit> _changeSubj = new();
    private readonly Subject<Unit> _resetSubj = new();
    private readonly Subject<Unit> _selectSubj = new();
    private readonly Subject<Unit> _rectAOISubj = new();
    private readonly Subject<Unit> _circleAOISubj = new();
    private readonly Subject<Unit> _polygonAOISubj = new();
    private readonly Subject<Unit> _routeSubj = new();
    private readonly Subject<Unit> _translateSubj = new();
    private readonly Subject<Unit> _scaleSubj = new();
    private readonly Subject<Unit> _rotateSubj = new();
    private readonly Subject<Unit> _editSubj = new();
    private readonly Subject<Unit> _drawPointSubj = new();
    private readonly Subject<Unit> _drawRectSubj = new();
    private readonly Subject<Unit> _drawCircleSubj = new();
    private readonly Subject<Unit> _drawPolygonSubj = new();

    public MapState()
    {
        _machine = new StateMachine<States, Trigger>(() => _state, s => _state = s);

        _machine.Configure(States.Default)
            .OnEntry(() => _resetSubj.OnNext(Unit.Default))
            .Permit(Trigger.Selectable, States.Select)
            .Permit(Trigger.RectAOIDrawing, States.RectAOI)
            .Permit(Trigger.CircleAOIDrawing, States.CircleAOI)
            .Permit(Trigger.PolygonAOIDrawing, States.PolygonAOI)
            .Permit(Trigger.RouteDrawing, States.Route)
            .Permit(Trigger.Translating, States.Translate)
            .Permit(Trigger.Rotating, States.Rotate)
            .Permit(Trigger.Scaling, States.Scale)
            .Permit(Trigger.Editing, States.Edit)
            .Permit(Trigger.PointDrawing, States.DrawPoint)
            .Permit(Trigger.RectDrawing, States.DrawRect)
            .Permit(Trigger.CircleDrawing, States.DrawCircle)
            .Permit(Trigger.PolygonDrawing, States.DrawPolygon);

        _machine
            .Configure(States.Select)
            .OnEntry(() => _selectSubj.OnNext(Unit.Default))
            .Permit(Trigger.Reset, States.Default)
            .Permit(Trigger.Selectable, States.Default);

        _machine
            .Configure(States.Translate)
            .OnEntry(() => _translateSubj.OnNext(Unit.Default))
            .Permit(Trigger.Reset, States.Default)
            .Permit(Trigger.Translating, States.Default);

        _machine.Configure(States.RectAOI)
            .OnEntry(() => _rectAOISubj.OnNext(Unit.Default))
            .Permit(Trigger.Reset, States.Default)
            .Permit(Trigger.RectAOIDrawing, States.Default);

        _machine.Configure(States.CircleAOI)
            .OnEntry(() => _circleAOISubj.OnNext(Unit.Default))
            .Permit(Trigger.Reset, States.Default)
            .Permit(Trigger.CircleAOIDrawing, States.Default);

        _machine.Configure(States.PolygonAOI)
            .OnEntry(() => _polygonAOISubj.OnNext(Unit.Default))
            .Permit(Trigger.Reset, States.Default)
            .Permit(Trigger.PolygonAOIDrawing, States.Default);

        _machine.Configure(States.Route)
            .OnEntry(() => _routeSubj.OnNext(Unit.Default))
            .Permit(Trigger.Reset, States.Default)
            .Permit(Trigger.RouteDrawing, States.Default);

        _machine.Configure(States.Rotate)
            .OnEntry(() => _rotateSubj.OnNext(Unit.Default))
            .Permit(Trigger.Reset, States.Default)
            .Permit(Trigger.Rotating, States.Default);

        _machine.Configure(States.Edit)
            .OnEntry(() => _editSubj.OnNext(Unit.Default))
            .Permit(Trigger.Reset, States.Default)
            .Permit(Trigger.Editing, States.Default);

        _machine.Configure(States.Scale)
            .OnEntry(() => _scaleSubj.OnNext(Unit.Default))
            .Permit(Trigger.Reset, States.Default)
            .Permit(Trigger.Scaling, States.Default);

        _machine.Configure(States.DrawPoint)
            .OnEntry(() => _drawPointSubj.OnNext(Unit.Default))
            .Permit(Trigger.Reset, States.Default)
            .Permit(Trigger.PointDrawing, States.Default);

        _machine.Configure(States.DrawRect)
            .OnEntry(() => _drawRectSubj.OnNext(Unit.Default))
            .Permit(Trigger.Reset, States.Default)
            .Permit(Trigger.RectDrawing, States.Default);

        _machine.Configure(States.DrawCircle)
            .OnEntry(() => _drawCircleSubj.OnNext(Unit.Default))
            .Permit(Trigger.Reset, States.Default)
            .Permit(Trigger.CircleDrawing, States.Default);

        _machine.Configure(States.DrawPolygon)
            .OnEntry(() => _drawPolygonSubj.OnNext(Unit.Default))
            .Permit(Trigger.Reset, States.Default)
            .Permit(Trigger.PolygonDrawing, States.Default);

        _machine.OnUnhandledTrigger(
            (state, trigger) =>
            {
                _machine.Fire(Trigger.Reset);

                _machine.Fire(trigger);
            });

        _machine.OnTransitioned((transition) => OnChange());
    }

    public States State => _machine.State;

    public IObservable<Unit> Change => _changeSubj.AsObservable();

    public bool IsInState(States state) => _machine.IsInState(state);

    private void OnChange() => _changeSubj.OnNext(Unit.Default);

    public IObservable<Unit> ResetObservable => _resetSubj.AsObservable();

    public IObservable<Unit> SelectObservable => _selectSubj.AsObservable();

    public IObservable<Unit> RectAOIObservable => _rectAOISubj.AsObservable();

    public IObservable<Unit> CircleAOIObservable => _circleAOISubj.AsObservable();

    public IObservable<Unit> PolygonAOIObservable => _polygonAOISubj.AsObservable();

    public IObservable<Unit> RouteObservable => _routeSubj.AsObservable();

    public IObservable<Unit> TranslateObservable => _translateSubj.AsObservable();

    public IObservable<Unit> ScaleObservable => _scaleSubj.AsObservable();

    public IObservable<Unit> RotateObservable => _rotateSubj.AsObservable();

    public IObservable<Unit> EditObservable => _editSubj.AsObservable();

    public IObservable<Unit> PointObservable => _drawPointSubj.AsObservable();

    public IObservable<Unit> RectObservable => _drawRectSubj.AsObservable();

    public IObservable<Unit> CircleObservable => _drawCircleSubj.AsObservable();

    public IObservable<Unit> PolygonObservable => _drawPolygonSubj.AsObservable();

    public void Select()
    {
        _machine.Fire(Trigger.Selectable);
    }

    public void RectAOI()
    {
        _machine.Fire(Trigger.RectAOIDrawing);
    }

    public void CircleAOI()
    {
        _machine.Fire(Trigger.CircleAOIDrawing);
    }

    public void PolygonAOI()
    {
        _machine.Fire(Trigger.PolygonAOIDrawing);
    }

    public void Route()
    {
        _machine.Fire(Trigger.RouteDrawing);
    }

    public void Translate()
    {
        _machine.Fire(Trigger.Translating);
    }

    public void Rotate()
    {
        _machine.Fire(Trigger.Rotating);
    }

    public void Scale()
    {
        _machine.Fire(Trigger.Scaling);
    }

    public void Edit()
    {
        _machine.Fire(Trigger.Editing);
    }

    public void Point()
    {
        _machine.Fire(Trigger.PointDrawing);
    }

    public void Rect()
    {
        _machine.Fire(Trigger.RectDrawing);
    }

    public void Circle()
    {
        _machine.Fire(Trigger.CircleDrawing);
    }

    public void Polygon()
    {
        _machine.Fire(Trigger.PolygonDrawing);
    }
}
