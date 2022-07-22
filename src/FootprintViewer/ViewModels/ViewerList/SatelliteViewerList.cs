using FootprintViewer.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class SatelliteViewerList : BaseViewerList<SatelliteViewModel>
    {
        private readonly IProvider<Satellite> _provider;

        public SatelliteViewerList(IProvider<Satellite> provider) : base()
        {
            _provider = provider;
        }

        protected override async Task<List<SatelliteViewModel>> LoadingAsync(IFilter<SatelliteViewModel>? filter = null)
        {
            return await _provider.GetValuesAsync(filter, s => new SatelliteViewModel(s));
        }

        protected override Task AddAsync(SatelliteViewModel? value) => throw new Exception();

        protected override Task RemoveAsync(SatelliteViewModel? value) => throw new Exception();
    }
}
