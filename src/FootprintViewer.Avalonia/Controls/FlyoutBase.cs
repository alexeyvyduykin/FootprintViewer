using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System;

namespace FootprintViewer.Avalonia.Controls
{
    public class FlyoutBase : ContentControl
    {
        public FlyoutBase()
        {
            Initialized += Flyout_Loaded;
        }

        private void Flyout_Loaded(object? sender, EventArgs e)
        {
            var b = Math.Sqrt(2 * ArrowSize * ArrowSize) / 2;

            TransformGroup transform = new TransformGroup();
            RelativePoint transformPoint = new RelativePoint();
            HorizontalAlignment horizontal = HorizontalAlignment.Left;
            VerticalAlignment vertical = VerticalAlignment.Top;

            double left = 0;
            double top = 0;
            double right = 0;
            double bottom = 0;

            var offset = ArrowOffset == 0.0 ? b : ArrowOffset;

            switch (Placement)
            {
                case FlyoutPlacementMode.Left:
                {
                    horizontal = HorizontalAlignment.Right;

                    vertical = VerticalAlignment.Top;

                    transform.Children.AddRange(new Transform[]
                    {
                        new RotateTransform(45),
                        new TranslateTransform(0, offset),
                    });

                    right = b;

                    transformPoint = new RelativePoint(new Point(1, 0), RelativeUnit.Relative);

                    break;
                }
                case FlyoutPlacementMode.Top:
                {
                    horizontal = HorizontalAlignment.Left;

                    vertical = VerticalAlignment.Bottom;

                    transform.Children.AddRange(new Transform[]
                    {
                        new RotateTransform(45),
                        new TranslateTransform(0, -b),
                        new TranslateTransform(offset - b, 0),
                    });

                    bottom = b;

                    transformPoint = new RelativePoint(new Point(0, 1), RelativeUnit.Relative);

                    break;
                }
                case FlyoutPlacementMode.Right:
                {
                    horizontal = HorizontalAlignment.Left;

                    vertical = VerticalAlignment.Top;

                    transform.Children.AddRange(new Transform[]
                    {
                        new RotateTransform(-45),
                        new TranslateTransform(0, offset),
                    });

                    left = b;

                    transformPoint = new RelativePoint(new Point(0, 0), RelativeUnit.Relative);

                    break;
                }
                case FlyoutPlacementMode.Bottom:
                {
                    horizontal = HorizontalAlignment.Left;

                    vertical = VerticalAlignment.Top;

                    transform.Children.AddRange(new Transform[]
                    {
                        new RotateTransform(45),
                        new TranslateTransform(0, -(ArrowSize - b)),
                        new TranslateTransform(offset - b, 0),
                    });

                    top = b;

                    transformPoint = new RelativePoint(new Point(0, 1), RelativeUnit.Relative);

                    break;
                }
                default:
                    break;
            }

            HorizontalAlignment = horizontal;

            VerticalAlignment = vertical;

            ArrowTransform = transform;

            ArrowOrigin = transformPoint;

            ArrowMargin = new Thickness(left, top, right, bottom);
        }

        public FlyoutPlacementMode Placement
        {
            get { return GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        public static readonly StyledProperty<FlyoutPlacementMode> PlacementProperty =
            AvaloniaProperty.Register<FlyoutBase, FlyoutPlacementMode>("Placement", FlyoutPlacementMode.Bottom);

        public double ArrowSize
        {
            get { return GetValue(ArrowSizeProperty); }
            set { SetValue(ArrowSizeProperty, value); }
        }

        public static readonly StyledProperty<double> ArrowSizeProperty =
            AvaloniaProperty.Register<FlyoutBase, double>("ArrowSize", 20);

        public RelativePoint ArrowOrigin
        {
            get { return GetValue(ArrowOriginProperty); }
            set { SetValue(ArrowOriginProperty, value); }
        }

        public static readonly StyledProperty<RelativePoint> ArrowOriginProperty =
            AvaloniaProperty.Register<FlyoutBase, RelativePoint>("ArrowOrigin");

        public ITransform? ArrowTransform
        {
            get { return GetValue(ArrowTransformProperty); }
            set { SetValue(ArrowTransformProperty, value); }
        }

        public static readonly StyledProperty<ITransform?> ArrowTransformProperty =
            AvaloniaProperty.Register<FlyoutBase, ITransform?>("ArrowTransform");

        public Thickness ArrowMargin
        {
            get { return GetValue(ArrowMarginProperty); }
            set { SetValue(ArrowMarginProperty, value); }
        }

        public static readonly StyledProperty<Thickness> ArrowMarginProperty =
            AvaloniaProperty.Register<FlyoutBase, Thickness>("ArrowMargin");

        public double ArrowOffset
        {
            get { return GetValue(ArrowOffsetProperty); }
            set { SetValue(ArrowOffsetProperty, value); }
        }

        public static readonly StyledProperty<double> ArrowOffsetProperty =
            AvaloniaProperty.Register<FlyoutBase, double>("ArrowOffset", 0.0);
    }
}
