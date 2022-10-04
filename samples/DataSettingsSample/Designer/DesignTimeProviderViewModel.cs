using DataSettingsSample.ViewModels;
using System.Collections.Generic;

namespace DataSettingsSample.Designer
{
    public class DesignTimeProviderViewModel : ProviderViewModel
    {
        public DesignTimeProviderViewModel() : base()
        {
            Header = "Footprints";

            Sources = new List<SourceViewModel>()
            {
                new SourceViewModel() { Name = "Source1" },
                new SourceViewModel() { Name = "Source2" },
                new SourceViewModel() { Name = "Source3" },
            };

            SourceBuilderItems = new List<SourceBuilderItemViewModel>
            {
                new SourceBuilderItemViewModel(){ Name = ".database" },
                new SourceBuilderItemViewModel(){ Name = ".json" },
                new SourceBuilderItemViewModel(){ Name = ".random" },
            };
        }
    }
}
