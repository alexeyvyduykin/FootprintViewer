using FootprintViewer.ViewModels;
using System.Collections.Generic;

namespace FootprintViewer.Designer
{
    public class DesignTimeProviderSettings : ProviderSettings
    {
        public DesignTimeProviderSettings() : base()
        {
            Type = ProviderType.GroundTargets;

            //AvailableSources = new List<ISourceBuilder>()
            //{
            //    new RandomSourceBuilder("RandomGroundTargets"),
            //    new DatabaseSourceBuilder(new DesignTimeData()),
            //};

            Sources.Add(new DatabaseSourceInfo()
            {
                Database = "database",
                Table = "GroundTargets"
            });
        }
    }
}
