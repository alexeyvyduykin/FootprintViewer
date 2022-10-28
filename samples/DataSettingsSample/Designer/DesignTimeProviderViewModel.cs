using DataSettingsSample.Data;
using DataSettingsSample.ViewModels;
using DataSettingsSample.ViewModels.Interfaces;
using Splat;
using System.Collections.Generic;

namespace DataSettingsSample.Designer
{
    public class DesignTimeProviderViewModel : ProviderViewModel
    {
        private static readonly Repository _repository = new DesignTimeData().GetService<Repository>()!;

        public DesignTimeProviderViewModel() : base(DbKeys.Footprints)
        {
            var source = _repository.GetSources(DbKeys.Footprints.ToString())[0];

            Header = DbKeys.Footprints.ToString();

            Sources = new List<ISourceViewModel>()
            {
                new SourceViewModel(source) { Name = "Source1" },
                new SourceViewModel(source) { Name = "Source2" },
                new SourceViewModel(source) { Name = "Source3" },
            };
        }
    }
}
