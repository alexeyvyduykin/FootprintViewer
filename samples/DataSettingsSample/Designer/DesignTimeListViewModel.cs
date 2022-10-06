using DataSettingsSample.ViewModels;
using System;

namespace DataSettingsSample.Designer
{
    public class DesignTimeListViewModel : ListViewModel
    {
        private static readonly double[] values = new[] { 542346565.3454, 56534343.6442, 9304038592.4331, 89023437112.9033, 7882343023.6033 };

        public DesignTimeListViewModel() : base(values) 
        {
            Load.Execute().Subscribe();
        }
    }
}
