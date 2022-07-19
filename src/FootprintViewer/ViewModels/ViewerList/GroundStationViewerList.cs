using FootprintViewer.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class GroundStationViewerList : BaseViewerList<GroundStationViewModel>
    {
        private readonly IProvider<GroundStation> _provider;

        public GroundStationViewerList(IProvider<GroundStation> provider) : base()
        {
            _provider = provider;
        }

        protected override async Task<List<GroundStationViewModel>> LoadingAsync(IFilter<GroundStationViewModel>? filter = null)
        {
            return await _provider.GetValuesAsync(filter, s => new GroundStationViewModel(s));
        }

        protected override Task AddAsync(GroundStationViewModel? value) => throw new Exception();

        protected override Task RemoveAsync(GroundStationViewModel? value) => throw new Exception();
    }
}
