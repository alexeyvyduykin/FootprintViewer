using controller = InteractivitySample.Input.Controller.Core;
using System.Windows;
using System.Windows.Input;
using M = Mapsui.Geometries;

namespace InteractivitySample
{
    public static class ConverterExtensions
    {
        public static M.Point ToScreenPoint(this Point pt)
        {
            return new M.Point(pt.X, pt.Y);
        }

        public static InteractivitySample.Input.MouseButton Convert(this System.Windows.Input.MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case System.Windows.Input.MouseButton.Left:
                    return InteractivitySample.Input.MouseButton.Left;
                case System.Windows.Input.MouseButton.Middle:
                    return InteractivitySample.Input.MouseButton.Middle;
                case System.Windows.Input.MouseButton.Right:
                    return InteractivitySample.Input.MouseButton.Right;
                default:
                    return InteractivitySample.Input.MouseButton.None;
            }
        }

        public static controller.MouseWheelEventArgs ToMouseWheelEventArgs(this System.Windows.Input.MouseWheelEventArgs e, IInputElement relativeTo)
        {
            return new controller.MouseWheelEventArgs
            {
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = Keyboard.Instance.GetModifierKeys(),
                Delta = e.Delta/*e.Delta.Y + e.Delta.X*/ * 120
            };
        }

        public static controller.MouseEventArgs ToMouseEventArgs(this System.Windows.Input.MouseEventArgs e, IInputElement relativeTo)
        {
            return new controller.MouseEventArgs
            {
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = e.KeyModifiers.ToModifierKeys()
            };
        }

        public static controller.MouseDownEventArgs ToMouseDownEventArgs(this MouseButtonEventArgs e, IInputElement relativeTo)
        {
            return new controller.MouseDownEventArgs
            {
                ChangedButton = e.ChangedButton.Convert(),
                ClickCount = e.ClickCount,
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = e.KeyModifiers.ToModifierKeys()
            };
        }

        public static controller.MouseEventArgs ToMouseReleasedEventArgs(this MouseButtonEventArgs e, IInputElement relativeTo)
        {
            return new controller.MouseEventArgs
            {
                Position = e.GetPosition(relativeTo).ToScreenPoint(),
                //ModifierKeys = e.KeyModifiers.ToModifierKeys()
            };
        }
    }
}
