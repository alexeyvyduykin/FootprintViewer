using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using System;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundTargetViewer : GroundTargetViewer
    {
        private static readonly DesignTimeData _designTimeData = new();

        public DesignTimeGroundTargetViewer() : base(_designTimeData)
        {
            var provider = _designTimeData.GetExistingService<IProvider<GroundTargetInfo>>();

            var list = new GroundTargetViewerList(provider);

            list.Loading.Execute().Subscribe();

            MainContent = list;
        }
    }
}
