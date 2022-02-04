﻿namespace FootprintViewer
{
    public static class MapCommands
    {
        static MapCommands()
        {
            Editing = new DelegateMapCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new EditingManipulator(view), args));
            HoverEditing = new DelegateMapCommand<MouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new HoverEditingManipulator(view), args));

            Editing2 = new DelegateMapCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new EditingManipulator2(view), args));
            HoverEditing2 = new DelegateMapCommand<MouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new HoverEditingManipulator2(view), args));

            Drawing2 = new DelegateMapCommand<MouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new DrawingManipulator2(view), args));
            HoverDrawing2 = new DelegateMapCommand<MouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new HoverDrawingManipulator2(view), args));
        }
    
        public static IViewCommand<MouseDownEventArgs> Editing { get; private set; }
        
        public static IViewCommand<MouseEventArgs> HoverEditing { get; private set; }

        public static IViewCommand<MouseDownEventArgs> Editing2 { get; private set; }

        public static IViewCommand<MouseEventArgs> HoverEditing2 { get; private set; }

        public static IViewCommand<MouseDownEventArgs> Drawing2 { get; private set; }

        public static IViewCommand<MouseEventArgs> HoverDrawing2 { get; private set; }
    }
}