using FootprintViewer.Data;
using ReactiveUI;

namespace FootprintViewer.ViewModels
{
    public class UserGeometryInfo : ReactiveObject
    {
        private readonly UserGeometry _geometry;

        public UserGeometryInfo(UserGeometry geometry)
        {
            _geometry = geometry;
        }

        public UserGeometry Geometry => _geometry;

        public string Name => _geometry.Name!;

        public UserGeometryType Type => _geometry.Type;
    }
}
