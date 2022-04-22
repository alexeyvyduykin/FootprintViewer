using InteractiveGeometry.UI.Input.Core;

namespace InteractiveGeometry.UI.Input
{
    public static class MapCommands
    {
        static MapCommands()
        {
            Default = new DelegateMapCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new DefaultManipulator(view), args));

            Editing = new DelegateMapCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new EditingManipulator(view), args));
            HoverEditing = new DelegateMapCommand<MouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new HoverEditingManipulator(view), args));

            Drawing = new DelegateMapCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new DrawingManipulator(view), args));
            HoverDrawing = new DelegateMapCommand<MouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new HoverDrawingManipulator(view), args));
        }

        public static IViewCommand<MouseDownEventArgs> Default { get; private set; }

        public static IViewCommand<MouseDownEventArgs> Editing { get; private set; }

        public static IViewCommand<MouseEventArgs> HoverEditing { get; private set; }

        public static IViewCommand<MouseDownEventArgs> Drawing { get; private set; }

        public static IViewCommand<MouseEventArgs> HoverDrawing { get; private set; }
    }
}