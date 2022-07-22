using FootprintViewer.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class FootprintViewerList : BaseViewerList<FootprintViewModel>
    {
        private readonly IProvider<Footprint> _provider;

        public FootprintViewerList(IProvider<Footprint> provider) : base()
        {
            _provider = provider;
        }

        protected override async Task<List<FootprintViewModel>> LoadingAsync(IFilter<FootprintViewModel>? filter = null)
        {
            return await _provider.GetValuesAsync(filter, s => new FootprintViewModel(s));
        }

        protected override Task AddAsync(FootprintViewModel? value) => throw new Exception();

        protected override Task RemoveAsync(FootprintViewModel? value) => throw new Exception();
    }
}
