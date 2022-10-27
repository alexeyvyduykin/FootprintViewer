using DataSettingsSample.ViewModels;
using DataSettingsSample.ViewModels.Interfaces;
using System.Collections.Generic;

namespace DataSettingsSample.Designer
{
    public class DesignTimeProviderViewModel : ProviderViewModel
    {
        public DesignTimeProviderViewModel() : base()
        {
            Header = "Footprints";

            Sources = new List<ISourceViewModel>()
            {
                new SourceViewModel() { Name = "Source1" },
                new SourceViewModel() { Name = "Source2" },
                new SourceViewModel() { Name = "Source3" },
            };
        }
    }
}
