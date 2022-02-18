using FootprintViewer.ViewModels;
using System;

namespace FootprintViewer.Designer
{
    public class DesignTimeUserGeometryViewer : UserGeometryViewer
    {
        public DesignTimeUserGeometryViewer() : base(new DesignTimeData())
        {
            Loading.Execute().Subscribe();
        }
    }
}
