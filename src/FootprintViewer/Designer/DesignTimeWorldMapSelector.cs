using FootprintViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer.Designer
{
    public class DesignTimeWorldMapSelector : WorldMapSelector
    {
        public DesignTimeWorldMapSelector() : base(new DesignTimeData())
        {

        }
    }
}
