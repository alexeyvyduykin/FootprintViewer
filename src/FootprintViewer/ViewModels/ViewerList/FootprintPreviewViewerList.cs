using FootprintViewer.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class FootprintPreviewViewerList : BaseViewerList<FootprintPreviewInfo>
    {
        private readonly IProvider<FootprintPreview> _provider;

        public FootprintPreviewViewerList(IProvider<FootprintPreview> provider) : base()
        {
            _provider = provider;
        }

        protected override async Task<List<FootprintPreviewInfo>> LoadingAsync(IFilter<FootprintPreviewInfo>? filter = null)
        {
            return await _provider.GetValuesAsync(filter, s => new FootprintPreviewInfo(s));
        }

        protected override Task AddAsync(FootprintPreviewInfo? value) => throw new Exception();

        protected override Task RemoveAsync(FootprintPreviewInfo? value) => throw new Exception();
    }
}
