namespace FootprintViewer.StateMachines;

public interface IStateCommands
{
    void ResetCommand();

    void RectangleCommand();

    void PolygonCommand();

    void CircleCommand();

    void RouteCommand();

    void SelectCommand();

    void TranslateCommand();

    void RotateCommand();

    void ScaleCommand();

    void EditCommand();

    void DrawingPointCommand();

    void DrawingRectangleCommand();

    void DrawingCircleCommand();

    void DrawingPolygonCommand();
}
