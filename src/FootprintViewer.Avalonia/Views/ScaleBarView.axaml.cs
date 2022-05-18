using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Mapsui;
using Mapsui.Projections;
using Mapsui.Widgets.ScaleBar;
using System;

namespace FootprintViewer.Avalonia.Views
{
    public partial class ScaleBarView : UserControl
    {
        private readonly IBrush _brushScaleBar = Brushes.WhiteSmoke;
        private readonly double MaxWidthScaleBar = 100;
        private readonly double StrokeWidth = 2;
        private readonly double TickLength = 4;

        public ScaleBarView()
        {
            InitializeComponent();

            //   Update += ScaleBarView_Update;

            UserMapControlProperty.Changed.Subscribe(OnUserMapControlChanged);
        }


        public UserMapControl UserMapControl
        {
            get { return GetValue(UserMapControlProperty); }
            set { SetValue(UserMapControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Controller.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<UserMapControl> UserMapControlProperty =
            AvaloniaProperty.Register<ScaleBarView, UserMapControl>(nameof(UserMapControl));


        private static void OnUserMapControlChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var scaleBarView = (ScaleBarView)e.Sender;
            if (e.NewValue == null)
            {

            }
            else
            {

            }
        }

        private static void Viewport_ViewportChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MapControl_ViewportUpdate(object? sender, EventArgs e)
        {
            if (sender is IReadOnlyViewport viewport)
            {
                OnUpdate(viewport);
            }
        }

        //private void ScaleBarView_Update(object? sender, EventArgs e)
        //{
        //    if (sender is IReadOnlyViewport viewport)
        //    {
        //        OnUpdate(viewport);
        //    }
        //}

        // public event EventHandler<EventArgs> Update;

        private void OnUpdate(/*Map map,*/ IReadOnlyViewport viewport)
        {
            if (Design.IsDesignMode == false)
            {
                var unitConverter = MetricUnitConverter.Instance;

                var center = new MPoint(viewport.Center.X, viewport.Center.Y);

                // We have to calc the angle difference to the equator (angle = 0), 
                // because EPSG:3857 is only there 1 m. At othere angles, we
                // should calculate the correct length.                       
                //        var position = (MPoint)map.Transformation.Transform(map.CRS, "EPSG:4326", center); // clone or else you will transform the orginal viewport center

                var proj = new Projection();
                //var position = proj.Project(map.CRS, "EPSG:4326", center.X, center.Y);
                var position = proj.Project("EPSG:3857", "EPSG:4326", center.X, center.Y);


                // Calc ground resolution in meters per pixel of viewport for this latitude
                double groundResolution = viewport.Resolution * Math.Cos(position.Y / 180.0 * Math.PI);

                // Convert in units of UnitConverter
                groundResolution = groundResolution / unitConverter.MeterRatio;

                var scaleBarValues = unitConverter.ScaleBarValues;

                double scaleBarLength = 0;
                int scaleBarValue = 0;

                foreach (int value in scaleBarValues)
                {
                    scaleBarValue = value;
                    scaleBarLength = (float)(scaleBarValue / groundResolution);
                    if (scaleBarLength < MaxWidthScaleBar - 10)
                    {
                        break;
                    }
                }

                var scaleBarText = unitConverter.GetScaleText(scaleBarValue);

                TextBlockScale.Text = scaleBarText;

                DrawScaleBar(scaleBarLength);
            }
        }

        public void DrawScaleBar(double scaleBarLength)
        {
            CanvasScale.Children.Clear();

            // Draw lines

            // Get lines for scale bar
            var points = GetScaleBarLinePositions(scaleBarLength);

            if (points != null)
            {
                // Draw lines
                for (int i = 0; i < points.Length; i += 2)
                {
                    var line = new Line();
                    //line.X1 = points[i].X;
                    //line.Y1 = points[i].Y;
                    //line.X2 = points[i + 1].X;
                    //line.Y2 = points[i + 1].Y;

                    line.StartPoint = points[i];
                    line.EndPoint = points[i + 1];

                    line.Stroke = _brushScaleBar;
                    line.StrokeThickness = StrokeWidth;
                    //line.StrokeStartLineCap = PenLineCap.Square;
                    //line.StrokeEndLineCap = PenLineCap.Square;

                    line.StrokeLineCap = PenLineCap.Square;

                    CanvasScale.Children.Add(line);
                }
            }
        }

        public Point[] GetScaleBarLinePositions(double scaleBarLength)
        {
            var posY = -(DesiredSize.Height/* ActualHeight*/ - TickLength) / 2;

            double center1 = 0;// (MaxWidthScaleBar - scaleBarLength) / 2;
            // Top position is Y in the middle of scale bar line
            double top = posY + DesiredSize.Height/* ActualHeight*/;

            var points = new Point[6];
            points[0] = new Point(center1, top - TickLength);
            points[1] = new Point(center1, top);
            points[2] = new Point(center1, top);
            points[3] = new Point(center1 + scaleBarLength, top);
            points[4] = new Point(center1 + scaleBarLength, top);
            points[5] = new Point(center1 + scaleBarLength, top - TickLength);

            return points;
        }
    }
}
