using FootprintViewer.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class FootprintViewerList : BaseViewerList<FootprintInfo>
    {
        private readonly IProvider<Footprint> _provider;

        public FootprintViewerList(IProvider<Footprint> provider) : base()
        {
            _provider = provider;
        }

        protected override async Task<List<FootprintInfo>> LoadingAsync(IFilter<FootprintInfo>? filter = null)
        {
            return await _provider.GetValuesAsync(filter, s => new FootprintInfo(s));
        }

        protected override Task AddAsync(FootprintInfo? value) => throw new Exception();

        protected override Task RemoveAsync(FootprintInfo? value) => throw new Exception();
    }
}
