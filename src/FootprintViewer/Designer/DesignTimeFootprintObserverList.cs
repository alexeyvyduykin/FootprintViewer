using FootprintViewer.ViewModels;
using System;
using System.Linq;

namespace FootprintViewer.Designer
{
    public class DesignTimeFootprintObserverList : FootprintObserverList
    {
        public DesignTimeFootprintObserverList() : base()
        {
            var provider = new DesignDataFootprintProvider();

            UpdateAsync.Execute(() => provider.GetFootprints().Select(s => new FootprintInfo(s))).Subscribe();
        }
    }
}
