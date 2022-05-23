using Mapsui;
using Mapsui.Projections;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FootprintViewer.ViewModels
{
    public interface IMapNavigator
    {
        void ZoomIn();

        void ZoomOut();

        void SetFocusToCoordinate(double lon, double lat);

        void SetFocusToPoint(double x, double y);

        void SetFocusToPoint(MPoint center);

        INavigator? Navigator { get; set; }
    }

    public class MapNavigator : ReactiveObject, IMapNavigator
    {
        public void ZoomIn()
        {
            Navigator?.ZoomIn();
        }

        public void ZoomOut()
        {
            Navigator?.ZoomOut();
        }

        public void SetFocusToCoordinate(double lon, double lat)
        {
            if (Navigator != null)
            {
                var (x, y) = SphericalMercator.FromLonLat(lon, lat);

                Navigator.CenterOn(x, y);
            }
        }

        public void SetFocusToPoint(double x, double y) => Navigator?.CenterOn(x, y);

        public void SetFocusToPoint(MPoint center) => Navigator?.CenterOn(center);

        [Reactive]
        public INavigator? Navigator { get; set; }
    }
}
