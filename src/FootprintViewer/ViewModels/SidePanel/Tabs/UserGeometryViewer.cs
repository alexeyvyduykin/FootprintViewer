using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class UserGeometryViewer : SidePanelTab
    {
        private readonly IEditableProvider<UserGeometryInfo> _provider;
        private bool _firstLoading = true;

        public UserGeometryViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<IEditableProvider<UserGeometryInfo>>();

            Title = "Пользовательская геометрия";

            ViewerList = ViewerListBuilder.CreateViewerList(_provider);

            // First loading

            this.WhenAnyValue(s => s.IsActive)
                .Where(s => s == true && _firstLoading == true)
                .Select(_ => (IFilter<UserGeometryInfo>?)null)
                .InvokeCommand(ViewerList.Loading);

            ViewerList.Loading.Subscribe(_ => _firstLoading = false);

            // Updating

            _provider.Update.Select(_ => (IFilter<UserGeometryInfo>?)null).InvokeCommand(ViewerList.Loading);
        }

        public async Task<List<UserGeometryInfo>> GetUserGeometryInfoAsync(string name)
        {
            return await _provider.GetValuesAsync(new NameFilter<UserGeometryInfo>(new[] { name }));
        }

        [Reactive]
        public IViewerList<UserGeometryInfo> ViewerList { get; private set; }
    }
}
