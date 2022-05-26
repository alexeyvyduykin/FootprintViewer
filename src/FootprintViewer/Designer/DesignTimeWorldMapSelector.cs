using FootprintViewer.ViewModels;
using System;

namespace FootprintViewer.Designer
{
    public class DesignTimeWorldMapSelector : WorldMapSelector
    {
        public DesignTimeWorldMapSelector() : base(new DesignTimeData())
        {
            Loading.Execute().Subscribe();
        }
    }
}
