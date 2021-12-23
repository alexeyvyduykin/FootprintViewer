using InteractivitySample.Input.Controller.Core;

namespace InteractivitySample.Input.Controller
{
    public static class MapCommands
    {
        static MapCommands()
        {
            Editing = new DelegateMapCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new EditingManipulator(view), args));
            HoverEditing = new DelegateMapCommand<MouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new HoverEditingManipulator(view), args));

            DrawingRectangle = new DelegateMapCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new DrawingRectangleManipulator(view), args));
            HoverDrawingRectangle = new DelegateMapCommand<MouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new HoverDrawingRectangleManipulator(view), args));

            DrawingCircle = new DelegateMapCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new DrawingCircleManipulator(view), args));
            HoverDrawingCircle = new DelegateMapCommand<MouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new HoverDrawingCircleManipulator(view), args));

            DrawingPolygon = new DelegateMapCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new DrawingPolygonManipulator(view), args));
            HoverDrawingPolygon = new DelegateMapCommand<MouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new HoverDrawingPolygonManipulator(view), args));

            DrawingRoute = new DelegateMapCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new DrawingRouteManipulator(view), args));
            HoverDrawingRoute = new DelegateMapCommand<MouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new HoverDrawingLineManipulator(view), args));
        }

        public static IViewCommand<MouseDownEventArgs> Editing { get; private set; }
        
        public static IViewCommand<MouseEventArgs> HoverEditing { get; private set; }

        public static IViewCommand<MouseDownEventArgs> DrawingRectangle { get; private set; }

        public static IViewCommand<MouseEventArgs> HoverDrawingRectangle { get; private set; }

        public static IViewCommand<MouseDownEventArgs> DrawingCircle { get; private set; }

        public static IViewCommand<MouseEventArgs> HoverDrawingCircle { get; private set; }

        public static IViewCommand<MouseDownEventArgs> DrawingPolygon { get; private set; }

        public static IViewCommand<MouseEventArgs> HoverDrawingPolygon { get; private set; }

        public static IViewCommand<MouseDownEventArgs> DrawingRoute { get; private set; }

        public static IViewCommand<MouseEventArgs> HoverDrawingRoute { get; private set; }
    }
}