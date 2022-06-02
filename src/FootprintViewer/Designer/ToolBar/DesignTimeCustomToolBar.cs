using FootprintViewer.ViewModels;
using System;

namespace FootprintViewer.Designer
{
    public class DesignTimeCustomToolBar : CustomToolBar
    {
        private static readonly DesignTimeData _designTimeData = new();

        public DesignTimeCustomToolBar() : base(_designTimeData)
        {
            _designTimeData.GetExistingService<WorldMapSelector>().Loading.Execute().Subscribe();
        }
    }
}
