using FootprintViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundTargetViewerList : GroundTargetViewerList
    {
        public DesignTimeGroundTargetViewerList()
        {
            var provider = new DesignDataGroundTargetProvider();

            UpdateAsync.Execute(() => provider.GetGroundTargets().Select(s => new GroundTargetInfo(s))).Subscribe();
        }
    }
}
