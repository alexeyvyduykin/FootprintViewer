using ToolBarSample.Models;

namespace ToolBarSample.ViewModels;

public class ToolBarViewModel : ViewModelBase
{
    public ToolBarViewModel(MapState mapState)
    {
        AddRectangle = new ToolCheck(
            mapState.Change,
            () => mapState.RectAOI(),
            () => mapState.IsInState(States.RectAOI))
        {
            Tag = "AddRectangle",
        };

        AddPolygon = new ToolCheck(
            mapState.Change,
            () => mapState.PolygonAOI(),
            () => mapState.IsInState(States.PolygonAOI))
        {
            Tag = "AddPolygon",
        };

        AddCircle = new ToolCheck(
            mapState.Change,
            () => mapState.CircleAOI(),
            () => mapState.IsInState(States.CircleAOI))
        {
            Tag = "AddCircle",
        };

        //AOICollection = new ToolCollection();
        //AOICollection.AddItem(AddRectangle);
        //AOICollection.AddItem(AddPolygon);
        //AOICollection.AddItem(AddCircle);

        RouteDistance = new ToolCheck(
            mapState.Change,
            () => mapState.Route(),
            () => mapState.IsInState(States.Route))
        {
            Tag = "Route",
        };

        SelectGeometry = new ToolCheck(
            mapState.Change,
            () => mapState.Select(),
            () => mapState.IsInState(States.Select))
        {
            Tag = "Select",
        };

        Point = new ToolCheck(
            mapState.Change,
            () => mapState.Point(),
            () => mapState.IsInState(States.DrawPoint))
        {
            Tag = "Point",
        };

        Rectangle = new ToolCheck(
            mapState.Change,
            () => mapState.Rect(),
            () => mapState.IsInState(States.DrawRect))
        {
            Tag = "Rectangle",
        };

        Circle = new ToolCheck(
            mapState.Change,
            () => mapState.Circle(),
            () => mapState.IsInState(States.DrawCircle))
        {
            Tag = "Circle",
        };

        Polygon = new ToolCheck(
            mapState.Change,
            () => mapState.Polygon(),
            () => mapState.IsInState(States.DrawPolygon))
        {
            Tag = "Polygon",
        };

        //GeometryCollection = new ToolCollection();
        //GeometryCollection.AddItem(Point);
        //GeometryCollection.AddItem(Rectangle);
        //GeometryCollection.AddItem(Circle);
        //GeometryCollection.AddItem(Polygon);

        TranslateGeometry = new ToolCheck(
            mapState.Change,
            () => mapState.Translate(),
            () => mapState.IsInState(States.Translate))
        {
            Tag = "Translate",
        };

        RotateGeometry = new ToolCheck(
            mapState.Change,
            () => mapState.Rotate(),
            () => mapState.IsInState(States.Rotate))
        {
            Tag = "Rotate",
        };

        ScaleGeometry = new ToolCheck(
            mapState.Change,
            () => mapState.Scale(),
            () => mapState.IsInState(States.Scale))
        {
            Tag = "Scale",
        };

        EditGeometry = new ToolCheck(
            mapState.Change,
            () => mapState.Edit(),
            () => mapState.IsInState(States.Edit))
        {
            Tag = "Edit",
        };
    }

    //public IToolCollection AOICollection { get; }

    public ToolCheck RouteDistance { get; }

    public ToolCheck SelectGeometry { get; }

    //public IToolCollection GeometryCollection { get; }

    public ToolCheck TranslateGeometry { get; }

    public ToolCheck RotateGeometry { get; }

    public ToolCheck ScaleGeometry { get; }

    public ToolCheck EditGeometry { get; }

    public ToolCheck AddRectangle { get; }

    public ToolCheck AddPolygon { get; }

    public ToolCheck AddCircle { get; }

    public ToolCheck Point { get; }

    public ToolCheck Rectangle { get; }

    public ToolCheck Circle { get; }

    public ToolCheck Polygon { get; }
}
