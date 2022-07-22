using FootprintViewer.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class FootprintPreviewViewerList : BaseViewerList<FootprintPreviewViewModel>
    {
        private readonly IProvider<FootprintPreview> _provider;

        public FootprintPreviewViewerList(IProvider<FootprintPreview> provider) : base()
        {
            _provider = provider;
        }

        protected override async Task<List<FootprintPreviewViewModel>> LoadingAsync(IFilter<FootprintPreviewViewModel>? filter = null)
        {
            return await _provider.GetValuesAsync(filter, s => new FootprintPreviewViewModel(s));
        }

        protected override Task AddAsync(FootprintPreviewViewModel? value) => throw new Exception();

        protected override Task RemoveAsync(FootprintPreviewViewModel? value) => throw new Exception();
    }
}
