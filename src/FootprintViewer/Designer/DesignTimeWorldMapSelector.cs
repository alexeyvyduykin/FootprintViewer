using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using System;

namespace FootprintViewer.Designer
{
    public class DesignTimeWorldMapSelector : WorldMapSelector
    {
        private static readonly DesignTimeData _designTimeData = new();

        public DesignTimeWorldMapSelector() : base(_designTimeData)
        {
            var mapProvider = _designTimeData.GetExistingService<IProvider<MapResource>>();

            mapProvider.Loading.Execute().Subscribe();
        }
    }
}
