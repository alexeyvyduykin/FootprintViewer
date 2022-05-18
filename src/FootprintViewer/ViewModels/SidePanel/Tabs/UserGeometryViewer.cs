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
    public class UserGeometryList : ViewerList<UserGeometryInfo>
    {
        public UserGeometryList(IEditableProvider<UserGeometryInfo> provider) : base(provider)
        {

        }
    }

    public class UserGeometryViewer : SidePanelTab
    {
        private readonly IEditableProvider<UserGeometryInfo> _provider;

        public UserGeometryViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<IEditableProvider<UserGeometryInfo>>();

            Title = "Пользовательская геометрия";

            ViewerList = new UserGeometryList(_provider);

            _provider.Update.Select(_ => (IFilter<UserGeometryInfo>?)null).InvokeCommand(ViewerList.Loading);

            this.WhenAnyValue(s => s.IsActive)
                .Where(s => s == true)
                .Select(_ => (IFilter<UserGeometryInfo>?)null)
                .InvokeCommand(ViewerList.Loading);
        }

        public async Task<List<UserGeometryInfo>> GetUserGeometryInfoAsync(string name)
        {
            return await _provider.GetValuesAsync(new NameFilter<UserGeometryInfo>(new[] { name }));
        }

        [Reactive]
        public IViewerList<UserGeometryInfo> ViewerList { get; private set; }
    }
}
