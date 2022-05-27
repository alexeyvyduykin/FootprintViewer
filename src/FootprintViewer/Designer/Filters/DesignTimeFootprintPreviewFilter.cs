using FootprintViewer.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace FootprintViewer.Designer
{
    public class DesignTimeFootprintPreviewFilter : SceneSearchFilter
    {
        public DesignTimeFootprintPreviewFilter() : base(new DesignTimeData())
        {
            Init.Execute().Subscribe();

            //FromDate = DateTime.Today.AddDays(-1);
            //ToDate = DateTime.Today.AddDays(1);
            //Sensors = new ObservableCollection<Sensor>(new[]
            //{
            //    new Sensor() { Name = "Satellite1 SNS-1" },
            //    new Sensor() { Name = "Satellite1 SNS-2" },
            //    new Sensor() { Name = "Satellite2 SNS-1" },
            //    new Sensor() { Name = "Satellite3 SNS-1" },
            //    new Sensor() { Name = "Satellite3 SNS-2" },
            //});
        }
    }
}
