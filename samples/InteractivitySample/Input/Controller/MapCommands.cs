﻿using InteractivitySample.Input.Controller.Core;

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
        }

        public static IViewCommand<MouseDownEventArgs> Editing { get; private set; }
        
        public static IViewCommand<MouseEventArgs> HoverEditing { get; private set; }

        public static IViewCommand<MouseDownEventArgs> DrawingRectangle { get; private set; }

        public static IViewCommand<MouseEventArgs> HoverDrawingRectangle { get; private set; }
    }
}