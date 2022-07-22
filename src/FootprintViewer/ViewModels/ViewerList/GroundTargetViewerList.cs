using FootprintViewer.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class GroundTargetViewerList : BaseViewerList<GroundTargetViewModel>
    {
        private readonly IProvider<GroundTarget> _provider;

        public GroundTargetViewerList(IProvider<GroundTarget> provider) : base()
        {
            _provider = provider;
        }

        protected override async Task<List<GroundTargetViewModel>> LoadingAsync(IFilter<GroundTargetViewModel>? filter = null)
        {
            return await _provider.GetValuesAsync(filter, s => new GroundTargetViewModel(s));
        }

        protected override Task AddAsync(GroundTargetViewModel? value) => throw new Exception();

        protected override Task RemoveAsync(GroundTargetViewModel? value) => throw new Exception();
    }
}
