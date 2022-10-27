using DataSettingsSample.ViewModels;
using DataSettingsSample.ViewModels.Interfaces;
using System.Collections.Generic;

namespace DataSettingsSample.Designer
{
    public class DesignTimeProviderViewModel : ProviderViewModel
    {
        public DesignTimeProviderViewModel() : base(Data.DbKeys.Footprints)
        {
            Header = "Footprints";

            Sources = new List<ISourceViewModel>()
            {
                new CustomSourceViewModel(null) { Name = "Source1" },
                new CustomSourceViewModel(null) { Name = "Source2" },
                new CustomSourceViewModel(null) { Name = "Source3" },
            };
        }
    }
}
