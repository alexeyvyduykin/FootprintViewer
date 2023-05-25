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
    private readonly Subject<Unit> _callbackSubj = new();

    public MapState()
    {
        _machine = new StateMachine<States, Triggers>(() => _state, s => _state = s);
    }

    public void Configure(IStateCommands commands)
    {
        _machine.Configure(States.Default)
            .OnEntry(commands.ResetCommand)
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
            .OnEntry(commands.SelectCommand)
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.Selectable, States.Default);

        _machine
            .Configure(States.Translate)
            .OnEntry(commands.TranslateCommand)
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.Translating, States.Default);

        _machine.Configure(States.DrawRectangleAoI)
            .OnEntry(commands.RectangleCommand)
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.RectAOIDrawing, States.Default);

        _machine.Configure(States.DrawCircleAoI)
            .OnEntry(commands.CircleCommand)
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.CircleAOIDrawing, States.Default);

        _machine.Configure(States.DrawPolygonAoI)
            .OnEntry(commands.PolygonCommand)
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.PolygonAOIDrawing, States.Default);

        _machine.Configure(States.DrawRoute)
            .OnEntry(commands.RouteCommand)
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.RouteDrawing, States.Default);

        _machine.Configure(States.Rotate)
            .OnEntry(commands.RotateCommand)
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.Rotating, States.Default);

        _machine.Configure(States.Edit)
            .OnEntry(commands.EditCommand)
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.Editing, States.Default);

        _machine.Configure(States.Scale)
            .OnEntry(commands.ScaleCommand)
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.Scaling, States.Default);

        _machine.Configure(States.DrawPoint)
            .OnEntry(commands.DrawingPointCommand)
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.PointDrawing, States.Default);

        _machine.Configure(States.DrawRectangle)
            .OnEntry(commands.DrawingRectangleCommand)
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.RectDrawing, States.Default);

        _machine.Configure(States.DrawCircle)
            .OnEntry(commands.DrawingCircleCommand)
            .Permit(Triggers.Reset, States.Default)
            .Permit(Triggers.CircleDrawing, States.Default);

        _machine.Configure(States.DrawPolygon)
            .OnEntry(commands.DrawingPolygonCommand)
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

    public IObservable<Unit> Callback => _callbackSubj.AsObservable();

    public bool IsInState(States state) => _machine.IsInState(state);

    private void OnChange() => _callbackSubj.OnNext(Unit.Default);

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
