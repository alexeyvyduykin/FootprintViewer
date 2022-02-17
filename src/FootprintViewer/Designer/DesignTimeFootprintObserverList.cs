using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using System;
using System.Linq;

namespace FootprintViewer.Designer
{
    public class DesignTimeFootprintObserverList : FootprintObserverList
    {
        public DesignTimeFootprintObserverList() : base(new DesignTimeData().GetExistingService<FootprintProvider>())
        {          
            Loading.Execute().Subscribe();
        }
    }
}
