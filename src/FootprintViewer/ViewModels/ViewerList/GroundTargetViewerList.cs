using FootprintViewer.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class GroundTargetViewerList : BaseViewerList<GroundTargetInfo>
    {
        private readonly IProvider<GroundTarget> _provider;

        public GroundTargetViewerList(IProvider<GroundTarget> provider) : base()
        {
            _provider = provider;
        }

        protected override async Task<List<GroundTargetInfo>> LoadingAsync(IFilter<GroundTargetInfo>? filter = null)
        {
            return await _provider.GetValuesAsync(filter, s => new GroundTargetInfo(s));
        }

        protected override Task AddAsync(GroundTargetInfo? value) => throw new Exception();

        protected override Task RemoveAsync(GroundTargetInfo? value) => throw new Exception();
    }
}
