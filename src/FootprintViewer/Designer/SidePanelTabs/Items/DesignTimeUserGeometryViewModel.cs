using FootprintViewer.Data;
using FootprintViewer.ViewModels.SidePanel.Items;
using System;

namespace FootprintViewer.Designer;

public class DesignTimeUserGeometryViewModel : UserGeometryViewModel
{
    private static readonly Random _random = new();

    public DesignTimeUserGeometryViewModel() : base(BuildModel())
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
