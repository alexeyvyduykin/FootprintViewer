using FootprintViewer.ViewModels;
using System;

namespace FootprintViewer.Designer
{
    public class DesignTimeFootprintInfoFilter : FootprintObserverFilter
    {
        public DesignTimeFootprintInfoFilter() : base(new DesignTimeData()) 
        {
            Init.Execute().Subscribe();
        }
    }
}
