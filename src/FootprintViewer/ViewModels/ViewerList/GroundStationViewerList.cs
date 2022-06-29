using FootprintViewer.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class GroundStationViewerList : BaseViewerList<GroundStationInfo>
    {
        private readonly IProvider<GroundStation> _provider;

        public GroundStationViewerList(IProvider<GroundStation> provider) : base()
        {
            _provider = provider;
        }

        protected override async Task<List<GroundStationInfo>> LoadingAsync(IFilter<GroundStationInfo>? filter = null)
        {
            return await _provider.GetValuesAsync(filter, s => new GroundStationInfo(s));
        }

        protected override Task AddAsync(GroundStationInfo? value) => throw new Exception();

        protected override Task RemoveAsync(GroundStationInfo? value) => throw new Exception();
    }
}
