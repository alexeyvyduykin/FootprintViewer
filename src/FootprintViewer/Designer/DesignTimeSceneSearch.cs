using DynamicData;
using FootprintViewer.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FootprintViewer.Designer
{
    public class DesignTimeSceneSearch : SceneSearch
    {
        public DesignTimeSceneSearch() : base(new DesignTimeData())
        {

        }
    }
}
