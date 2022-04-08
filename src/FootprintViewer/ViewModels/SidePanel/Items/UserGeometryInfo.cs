using FootprintViewer.Data;
using ReactiveUI;

namespace FootprintViewer.ViewModels
{
    public class UserGeometryInfo : ReactiveObject, ISelectorItem
    {
        private readonly UserGeometry _geometry;

        public UserGeometryInfo(UserGeometry geometry)
        {
            _geometry = geometry;
        }

        public string GetKey() => _geometry.Type.ToString();

        public UserGeometry Geometry => _geometry;

        public string Name => _geometry.Name!;

        public UserGeometryType Type => _geometry.Type;
    }
}
