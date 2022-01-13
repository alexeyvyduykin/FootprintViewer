using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Mapsui.Widgets.ScaleBar;
using Mapsui;
using System;

namespace FootprintViewer.Avalonia.Views
{
    public partial class ScaleBarView : UserControl
    {
        private readonly IBrush _brushScaleBar = Brushes.WhiteSmoke;
        private readonly double MaxWidthScaleBar = 100;
        private readonly double StrokeWidth = 2;
        private readonly double TickLength = 4;
        private readonly TextBlock _textBlockScale;
        private readonly Canvas _canvasScale;

        public ScaleBarView()
        {
            InitializeComponent();

            _textBlockScale = this.FindControl<TextBlock>("TextBlockScale");
            _canvasScale = this.FindControl<Canvas>("CanvasScale");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void Update(Map map, Viewport viewport)
        {
            if (Design.IsDesignMode == false)
            {
                var unitConverter = MetricUnitConverter.Instance;

                var center = new Mapsui.Geometries.Point(viewport.Center.X, viewport.Center.Y);

                // We have to calc the angle difference to the equator (angle = 0), 
                // because EPSG:3857 is only there 1 m. At othere angles, we
                // should calculate the correct length.                       
                var position = (Mapsui.Geometries.Point)map.Transformation.Transform(map.CRS, "EPSG:4326", center); // clone or else you will transform the orginal viewport center

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

                _textBlockScale.Text = scaleBarText;

                DrawScaleBar(scaleBarLength);
            }            
        }

        public void DrawScaleBar(double scaleBarLength)
        {
            _canvasScale.Children.Clear();

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

                    _canvasScale.Children.Add(line);
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
