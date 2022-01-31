using FootprintViewer.Data;
using ReactiveUI;

namespace FootprintViewer.ViewModels
{
    public class UserGeometryInfo : ReactiveObject
    {
        private readonly string? _name;
        private readonly UserGeometryType _type;

        public UserGeometryInfo(UserGeometry geometry)
        {
            _name = geometry.Name;
            _type = geometry.Type;
        }

        public string? Name => _name;

        public UserGeometryType Type => _type;
    }
}
