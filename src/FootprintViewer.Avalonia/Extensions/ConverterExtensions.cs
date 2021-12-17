using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using M = Mapsui.Geometries;
using FootprintViewer;
using Avalonia;
using avalonia = Avalonia.Input;
using Avalonia.Input;

namespace FootprintViewer.Avalonia
{
    public static class ConverterExtensions
    {
        public static M.Point ToScreenPoint(this Point pt)
        {
            return new M.Point(pt.X, pt.Y);
        }

        public static MouseButton Convert(this PointerUpdateKind state)
        {
            switch (state)
            {
                case PointerUpdateKind.LeftButtonPressed:
                    return MouseButton.Left;
                case PointerUpdateKind.MiddleButtonPressed:
                    return MouseButton.Middle;
                case PointerUpdateKind.RightButtonPressed:
                    return MouseButton.Right;
                case PointerUpdateKind.XButton1Pressed:
                    return MouseButton.XButton1;
                case PointerUpdateKind.XButton2Pressed:
                    return MouseButton.XButton2;
                case PointerUpdateKind.LeftButtonReleased:
                    return MouseButton.Left;
                case PointerUpdateKind.MiddleButtonReleased:
                    return MouseButton.Middle;
                case PointerUpdateKind.RightButtonReleased:
                    return MouseButton.Right;
                case PointerUpdateKind.XButton1Released:
                    return MouseButton.XButton1;
                case PointerUpdateKind.XButton2Released:
                    return MouseButton.XButton2;
                case PointerUpdateKind.Other:
                    return MouseButton.None;
                default:
                    return MouseButton.None;
            }
        }

        public static MouseWheelEventArgs ToMouseWheelEventArgs(this PointerWheelEventArgs e, IInputElement relativeTo)
        {
            return new MouseWheelEventArgs
            {
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = Keyboard.Instance.GetModifierKeys(),
                Delta = (int)(e.Delta.Y + e.Delta.X) * 120
            };
        }

        public static MouseEventArgs ToMouseEventArgs(this PointerEventArgs e, IInputElement relativeTo)
        {
            return new MouseEventArgs
            {
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = e.KeyModifiers.ToModifierKeys()
            };
        }

        public static MouseDownEventArgs ToMouseDownEventArgs(this PointerPressedEventArgs e, IInputElement relativeTo)
        {
            return new MouseDownEventArgs
            {
                ChangedButton = e.GetPointerPoint(null).Properties.PointerUpdateKind.Convert(),
                ClickCount = e.ClickCount,
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = e.KeyModifiers.ToModifierKeys()
            };
        }

        public static MouseEventArgs ToMouseReleasedEventArgs(this PointerReleasedEventArgs e, IInputElement relativeTo)
        {
            return new MouseEventArgs
            {
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = e.KeyModifiers.ToModifierKeys()
            };
        }
    }
}
