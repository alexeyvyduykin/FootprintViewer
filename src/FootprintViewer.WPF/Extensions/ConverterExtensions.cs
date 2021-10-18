using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using M = Mapsui.Geometries;
using FootprintViewer;

namespace FootprintViewer.WPF
{
    public static class ConverterExtensions
    {
        public static M.Point ToScreenPoint(this Point pt)
        {
            return new M.Point(pt.X, pt.Y);
        }

        public static MouseButton Convert(this System.Windows.Input.MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case System.Windows.Input.MouseButton.Left:
                    return MouseButton.Left;
                case System.Windows.Input.MouseButton.Middle:
                    return MouseButton.Middle;
                case System.Windows.Input.MouseButton.Right:
                    return MouseButton.Right;
                default:
                    return MouseButton.None;
            }
        }

        public static MouseWheelEventArgs ToMouseWheelEventArgs(this System.Windows.Input.MouseWheelEventArgs e, IInputElement relativeTo)
        {
            return new MouseWheelEventArgs
            {
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = Keyboard.Instance.GetModifierKeys(),
                Delta = e.Delta/*e.Delta.Y + e.Delta.X*/ * 120
            };
        }

        public static MouseEventArgs ToMouseEventArgs(this System.Windows.Input.MouseEventArgs e, IInputElement relativeTo)
        {
            return new MouseEventArgs
            {
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = e.KeyModifiers.ToModifierKeys()
            };
        }

        public static MouseDownEventArgs ToMouseDownEventArgs(this MouseButtonEventArgs e, IInputElement relativeTo)
        {
            return new MouseDownEventArgs
            {
                ChangedButton = e.ChangedButton.Convert(),
                ClickCount = e.ClickCount,
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = e.KeyModifiers.ToModifierKeys()
            };
        }

        public static MouseEventArgs ToMouseReleasedEventArgs(this MouseButtonEventArgs e, IInputElement relativeTo)
        {
            return new MouseEventArgs
            {
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = e.KeyModifiers.ToModifierKeys()
            };
        }
    }
}
