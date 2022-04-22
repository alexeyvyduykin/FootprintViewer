using Avalonia;
using InteractiveSample.Input;
using InteractiveSample.Input.Controller.Core;
using InteractiveSample.Layers;
using Mapsui;
using avalonia = Avalonia.Input;
using InteractiveGeometry;
using InteractiveSample.Input.Controller;
using InteractiveSample.Layers;
using System.Linq;
using Mapsui.Layers;
using BruTile.Wms;

namespace InteractiveSample
{
    public static class ConverterExtensions
    {
        public static MPoint ToScreenPoint(this Point pt)
        {
            return new MPoint(pt.X, pt.Y);
        }

        public static MouseButton Convert(this avalonia.PointerUpdateKind state)
        {
            switch (state)
            {
                case avalonia.PointerUpdateKind.LeftButtonPressed:
                    return MouseButton.Left;
                case avalonia.PointerUpdateKind.MiddleButtonPressed:
                    return MouseButton.Middle;
                case avalonia.PointerUpdateKind.RightButtonPressed:
                    return MouseButton.Right;
                case avalonia.PointerUpdateKind.XButton1Pressed:
                    return MouseButton.XButton1;
                case avalonia.PointerUpdateKind.XButton2Pressed:
                    return MouseButton.XButton2;
                case avalonia.PointerUpdateKind.LeftButtonReleased:
                    return MouseButton.Left;
                case avalonia.PointerUpdateKind.MiddleButtonReleased:
                    return MouseButton.Middle;
                case avalonia.PointerUpdateKind.RightButtonReleased:
                    return MouseButton.Right;
                case avalonia.PointerUpdateKind.XButton1Released:
                    return MouseButton.XButton1;
                case avalonia.PointerUpdateKind.XButton2Released:
                    return MouseButton.XButton2;
                case avalonia.PointerUpdateKind.Other:
                    return MouseButton.None;
                default:
                    return MouseButton.None;
            }
        }

        public static MouseWheelEventArgs ToMouseWheelEventArgs(this avalonia.PointerWheelEventArgs e, avalonia.IInputElement relativeTo)
        {
            return new MouseWheelEventArgs
            {
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = Keyboard.Instance.GetModifierKeys(),
                Delta = (int)(e.Delta.Y + e.Delta.X) * 120
            };
        }

        public static MouseEventArgs ToMouseEventArgs(this avalonia.PointerEventArgs e, avalonia.IInputElement relativeTo)
        {
            return new MouseEventArgs
            {
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = e.KeyModifiers.ToModifierKeys()
            };
        }

        public static MouseDownEventArgs ToMouseDownEventArgs(this avalonia.PointerPressedEventArgs e, avalonia.IInputElement relativeTo)
        {
            return new MouseDownEventArgs
            {
#pragma warning disable CS0618 // Тип или член устарел
                ChangedButton = e.GetPointerPoint(null).Properties.PointerUpdateKind.Convert(),
#pragma warning restore CS0618 // Тип или член устарел
                ClickCount = e.ClickCount,
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = e.KeyModifiers.ToModifierKeys()
            };
        }

        public static MouseEventArgs ToMouseReleasedEventArgs(this avalonia.PointerReleasedEventArgs e, avalonia.IInputElement relativeTo)
        {
            return new MouseEventArgs
            {
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = e.KeyModifiers.ToModifierKeys()
            };
        }
    }
}
