using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using System;

namespace FootprintViewer.Designer
{
    public class DesignTimeUserGeometryInfoList : ViewerList<UserGeometryInfo>
    {
        public DesignTimeUserGeometryInfoList() : base(new DesignTimeData().GetExistingService<IEditableProvider<UserGeometryInfo>>())
        {
            Loading.Execute().Subscribe();
        }
    }
}
