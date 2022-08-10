using FootprintViewer.ViewModels;
using System;

namespace FootprintViewer.Designer
{
    public class DesignTimeFootprintPreviewTab : FootprintPreviewTab
    {
        public DesignTimeFootprintPreviewTab() : base(new DesignTimeData())
        {
            IsActive = true;
        }
    }
}
