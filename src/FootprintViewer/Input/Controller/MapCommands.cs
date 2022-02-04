namespace FootprintViewer
{
    public static class MapCommands
    {
        static MapCommands()
        {
            DrawingLine = new DelegateMapCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new DrawingRouteManipulator(view), args));
            HoverDrawingLine = new DelegateMapCommand<MouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new HoverDrawingLineManipulator(view), args));

            DrawingPolygon = new DelegateMapCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new DrawingPolygonManipulator(view), args));
            HoverDrawingPolygon = new DelegateMapCommand<MouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new HoverDrawingPolygonManipulator(view), args));

            DrawingCircle = new DelegateMapCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new DrawingCircleManipulator(view), args));
            HoverDrawingCircle = new DelegateMapCommand<MouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new HoverDrawingCircleManipulator(view), args));

            Editing = new DelegateMapCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new EditingManipulator(view), args));
            HoverEditing = new DelegateMapCommand<MouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new HoverEditingManipulator(view), args));

            Editing2 = new DelegateMapCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new EditingManipulator2(view), args));
            HoverEditing2 = new DelegateMapCommand<MouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new HoverEditingManipulator2(view), args));

            Drawing2 = new DelegateMapCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new DrawingManipulator2(view), args));
            HoverDrawing2 = new DelegateMapCommand<MouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new HoverDrawingManipulator2(view), args));
        }

        public static IViewCommand<MouseDownEventArgs> DrawingLine { get; private set; }

        public static IViewCommand<MouseEventArgs> HoverDrawingLine { get; private set; }
        
        public static IViewCommand<MouseDownEventArgs> DrawingPolygon { get; private set; }

        public static IViewCommand<MouseEventArgs> HoverDrawingPolygon { get; private set; }

        public static IViewCommand<MouseDownEventArgs> DrawingCircle { get; private set; }

        public static IViewCommand<MouseEventArgs> HoverDrawingCircle { get; private set; }

        public static IViewCommand<MouseDownEventArgs> Editing { get; private set; }
        
        public static IViewCommand<MouseEventArgs> HoverEditing { get; private set; }

        public static IViewCommand<MouseDownEventArgs> Editing2 { get; private set; }

        public static IViewCommand<MouseEventArgs> HoverEditing2 { get; private set; }

        public static IViewCommand<MouseDownEventArgs> Drawing2 { get; private set; }

        public static IViewCommand<MouseEventArgs> HoverDrawing2 { get; private set; }
    }
}