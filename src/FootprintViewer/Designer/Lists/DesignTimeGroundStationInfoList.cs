using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using System;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundStationInfoList : ViewerList<GroundStationInfo>
    {
        public DesignTimeGroundStationInfoList() : base(new DesignTimeData().GetExistingService<IProvider<GroundStationInfo>>())
        {
            Loading.Execute().Subscribe();
        }
    }
}
