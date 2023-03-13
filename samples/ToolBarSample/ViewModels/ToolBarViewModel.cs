using ReactiveUI.Fody.Helpers;

namespace ToolBarSample.ViewModels;

public class ToolBarViewModel : ViewModelBase
{
    public ToolBarViewModel()
    {
        AddRectangle = new ToolCheck()
        {
            Tag = "AddRectangle",
        };

        AddPolygon = new ToolCheck()
        {
            Tag = "AddPolygon",
        };

        AddCircle = new ToolCheck()
        {
            Tag = "AddCircle",
        };

        //AOICollection = new ToolCollection();
        //AOICollection.AddItem(AddRectangle);
        //AOICollection.AddItem(AddPolygon);
        //AOICollection.AddItem(AddCircle);

        RouteDistance = new ToolCheck()
        {
            Tag = "Route",
        };

        SelectGeometry = new ToolCheck()
        {
            Tag = "Select",
        };

        Point = new ToolCheck()
        {
            Tag = "Point",
        };

        Rectangle = new ToolCheck()
        {
            Tag = "Rectangle",
        };

        Circle = new ToolCheck()
        {
            Tag = "Circle",
        };

        Polygon = new ToolCheck()
        {
            Tag = "Polygon",
        };

        //GeometryCollection = new ToolCollection();
        //GeometryCollection.AddItem(Point);
        //GeometryCollection.AddItem(Rectangle);
        //GeometryCollection.AddItem(Circle);
        //GeometryCollection.AddItem(Polygon);

        TranslateGeometry = new ToolCheck()
        {
            Tag = "Translate",
        };

        RotateGeometry = new ToolCheck()
        {
            Tag = "Rotate",
        };

        ScaleGeometry = new ToolCheck()
        {
            Tag = "Scale",
        };

        EditGeometry = new ToolCheck()
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

    [Reactive]
    public string ConsoleString { get; set; } = "Console";
}
