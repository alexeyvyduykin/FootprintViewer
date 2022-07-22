using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class UserGeometryTab : SidePanelTab
    {
        private readonly IEditableProvider<UserGeometry> _provider;

        public UserGeometryTab(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<IEditableProvider<UserGeometry>>();
            var viewModelFactory = dependencyResolver.GetExistingService<ViewModelFactory>();

            Title = "Пользовательская геометрия";

            ViewerList = viewModelFactory.CreateUserGeometryViewerList(_provider);

            _provider.Observable.Skip(1).Select(s => (IFilter<UserGeometryViewModel>?)null).InvokeCommand(ViewerList.Loading);

            // First loading

            this.WhenAnyValue(s => s.IsActive)
                .Take(1)
                .Where(active => active == true)
                .Select(_ => (IFilter<UserGeometryViewModel>?)null)
                .InvokeCommand(ViewerList.Loading);

            // Updating

            _provider.Update.Select(_ => (IFilter<UserGeometryViewModel>?)null).InvokeCommand(ViewerList.Loading);
        }

        public async Task<List<UserGeometryViewModel>> GetUserGeometryViewModelsAsync(string name)
        {
            return await _provider.GetValuesAsync<UserGeometryViewModel>(new NameFilter<UserGeometryViewModel>(new[] { name }), s => new UserGeometryViewModel(s));
        }

        [Reactive]
        public IViewerList<UserGeometryViewModel> ViewerList { get; private set; }
    }
}
