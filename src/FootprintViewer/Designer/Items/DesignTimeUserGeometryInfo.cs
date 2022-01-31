using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using System;

namespace FootprintViewer.Designer
{
    public class DesignTimeUserGeometryInfo : UserGeometryInfo
    {
        private static readonly Random _random = new Random();

        public DesignTimeUserGeometryInfo() : base(BuildModel())
        {

        }

        public static UserGeometry BuildModel()
        {         
            return new UserGeometry()
            {
                Name = $"UserGeometry{_random.Next(1, 101):000}",
                Type = (UserGeometryType)_random.Next(0, 4),
            };
        }
    }
}
