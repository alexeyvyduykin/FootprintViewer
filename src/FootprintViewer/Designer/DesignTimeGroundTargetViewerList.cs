using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using System;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundTargetViewerList //: GroundTargetViewerList
    {
        private static readonly DesignTimeData _designTimeData = new();

        public DesignTimeGroundTargetViewerList() //: base(_designTimeData.GetExistingService<IProvider<GroundTargetInfo>>())
        {
            //Loading.Execute(null).Subscribe();
        }
    }
}
