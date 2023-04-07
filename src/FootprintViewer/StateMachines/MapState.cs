using Stateless;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace FootprintViewer.StateMachines;

public class MapState
{
    private enum Triggers
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
    private readonly StateMachine<States, Triggers> _machine;
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
        _machine = new StateMachine<States, Triggers>(() => _state, s => _state = s);

        _machine.Configure(States.Default)
            .OnEntry(() => _resetSubj.OnNext(Unit.Default))
            .Permit(Triggers.Selectable, States.Select)
            .Permit(Triggers.RectAOIDrawing, States.DrawRectangleAoI)
            .Permit(Triggers.CircleAOIDrawing, States.DrawCircleAoI)
            .Permit(Triggers.PolygonAOIDrawing, States.DrawPolygonAoI)
            .Permit(Triggers.RouteDrawing, States.DrawRoute)
            .Permit(Triggers.Translating, States.Translate)
            .Permit(Triggers.Rotating, States.Rotate)
            .Permit(Triggers.Scaling, States.Scale)
            .Permit(Triggers.Editing, States.Edit)
            .Permit(Triggers.PointDrawing, States.DrawPoint)
            .Permit(Triggers.RectDrawing, States.DrawRectangle)
            .Permit(Triggers.CircleDrawing, States.DrawCircle)
            .Permit(Triggers.PolygonDrawing, States.DrawPolygon);

        _machine
            .Configure(States.Select)
            .OnEntry(() => _selectSubj.OnNext(Unit.Default))
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.Selectable, States.Default);

        _machine
            .Configure(States.Translate)
            .OnEntry(() => _translateSubj.OnNext(Unit.Default))
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.Translating, States.Default);

        _machine.Configure(States.DrawRectangleAoI)
            .OnEntry(() => _rectAOISubj.OnNext(Unit.Default))
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.RectAOIDrawing, States.Default);

        _machine.Configure(States.DrawCircleAoI)
            .OnEntry(() => _circleAOISubj.OnNext(Unit.Default))
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.CircleAOIDrawing, States.Default);

        _machine.Configure(States.DrawPolygonAoI)
            .OnEntry(() => _polygonAOISubj.OnNext(Unit.Default))
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.PolygonAOIDrawing, States.Default);

        _machine.Configure(States.DrawRoute)
            .OnEntry(() => _routeSubj.OnNext(Unit.Default))
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.RouteDrawing, States.Default);

        _machine.Configure(States.Rotate)
            .OnEntry(() => _rotateSubj.OnNext(Unit.Default))
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.Rotating, States.Default);

        _machine.Configure(States.Edit)
            .OnEntry(() => _editSubj.OnNext(Unit.Default))
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.Editing, States.Default);

        _machine.Configure(States.Scale)
            .OnEntry(() => _scaleSubj.OnNext(Unit.Default))
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.Scaling, States.Default);

        _machine.Configure(States.DrawPoint)
            .OnEntry(() => _drawPointSubj.OnNext(Unit.Default))
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.PointDrawing, States.Default);

        _machine.Configure(States.DrawRectangle)
            .OnEntry(() => _drawRectSubj.OnNext(Unit.Default))
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.RectDrawing, States.Default);

        _machine.Configure(States.DrawCircle)
            .OnEntry(() => _drawCircleSubj.OnNext(Unit.Default))
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.CircleDrawing, States.Default);

        _machine.Configure(States.DrawPolygon)
            .OnEntry(() => _drawPolygonSubj.OnNext(Unit.Default))
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.PolygonDrawing, States.Default);

        _machine.OnUnhandledTrigger(
            (state, trigger) =>
            {
                _machine.Fire(Triggers.Reset);

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

    public void Reset()
    {
        // TODO: issue (Default state -> Reset trigger)
        if (Equals(State, States.Default) == false)
        {
            _machine.Fire(Triggers.Reset);
        }
    }

    public void Select()
    {
        _machine.Fire(Triggers.Selectable);
    }

    public void RectAOI()
    {
        _machine.Fire(Triggers.RectAOIDrawing);
    }

    public void CircleAOI()
    {
        _machine.Fire(Triggers.CircleAOIDrawing);
    }

    public void PolygonAOI()
    {
        _machine.Fire(Triggers.PolygonAOIDrawing);
    }

    public void Route()
    {
        _machine.Fire(Triggers.RouteDrawing);
    }

    public void Translate()
    {
        _machine.Fire(Triggers.Translating);
    }

    public void Rotate()
    {
        _machine.Fire(Triggers.Rotating);
    }

    public void Scale()
    {
        _machine.Fire(Triggers.Scaling);
    }

    public void Edit()
    {
        _machine.Fire(Triggers.Editing);
    }

    public void Point()
    {
        _machine.Fire(Triggers.PointDrawing);
    }

    public void Rect()
    {
        _machine.Fire(Triggers.RectDrawing);
    }

    public void Circle()
    {
        _machine.Fire(Triggers.CircleDrawing);
    }

    public void Polygon()
    {
        _machine.Fire(Triggers.PolygonDrawing);
    }
}
