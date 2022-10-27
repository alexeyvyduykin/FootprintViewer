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
                new SourceViewModel(null) { Name = "Source1" },
                new SourceViewModel(null) { Name = "Source2" },
                new SourceViewModel(null) { Name = "Source3" },
            };
        }
    }
}
