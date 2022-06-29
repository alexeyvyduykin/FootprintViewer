using FootprintViewer.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class SatelliteViewerList : BaseViewerList<SatelliteInfo>
    {
        private readonly IProvider<Satellite> _provider;

        public SatelliteViewerList(IProvider<Satellite> provider) : base()
        {
            _provider = provider;
        }

        protected override async Task<List<SatelliteInfo>> LoadingAsync(IFilter<SatelliteInfo>? filter = null)
        {
            return await _provider.GetValuesAsync(filter, s => new SatelliteInfo(s));
        }

        protected override Task AddAsync(SatelliteInfo? value) => throw new Exception();

        protected override Task RemoveAsync(SatelliteInfo? value) => throw new Exception();
    }
}
