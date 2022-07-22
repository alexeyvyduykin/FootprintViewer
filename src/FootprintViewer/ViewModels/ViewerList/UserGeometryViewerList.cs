using FootprintViewer.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class UserGeometryViewerList : BaseViewerList<UserGeometryViewModel>
    {
        private readonly IProvider<UserGeometry> _provider;

        public UserGeometryViewerList(IProvider<UserGeometry> provider) : base()
        {
            _provider = provider;
        }

        protected override async Task<List<UserGeometryViewModel>> LoadingAsync(IFilter<UserGeometryViewModel>? filter = null)
        {
            return await _provider.GetValuesAsync(filter, s => new UserGeometryViewModel(s));
        }

        protected override async Task AddAsync(UserGeometryViewModel? value)
        {
            if (value != null && _provider is IEditableProvider<UserGeometry> editableProvider)
            {
                await editableProvider.AddAsync(value.Model);
            }
        }

        protected override async Task RemoveAsync(UserGeometryViewModel? value)
        {
            if (value != null && _provider is IEditableProvider<UserGeometry> editableProvider)
            {
                await editableProvider.RemoveAsync(value.Model);
            }
        }
    }
}
