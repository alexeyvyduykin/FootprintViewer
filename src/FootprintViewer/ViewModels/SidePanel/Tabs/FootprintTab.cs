using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class FootprintTab : SidePanelTab
    {
        public FootprintTab(IReadonlyDependencyResolver dependencyResolver)
        {
            var provider = dependencyResolver.GetExistingService<IProvider<Footprint>>();
            var viewModelFactory = dependencyResolver.GetExistingService<ViewModelFactory>();

            ViewerList = viewModelFactory.CreateFootprintViewerList(provider);

            Filter = new FootprintObserverFilter(dependencyResolver);

            Title = "Просмотр рабочей программы";

            ClickOnItem = ReactiveCommand.Create<FootprintViewModel?, FootprintViewModel?>(s => { ViewerList.ClickOnItem(s); return s; });

            provider.Observable.Skip(1).Select(s => Filter).InvokeCommand(ViewerList.Loading);

            // First loading

            this.WhenAnyValue(s => s.IsActive)
                .Take(1)
                .Where(active => active == true)
                .Select(_ => Filter)
                .InvokeCommand(ViewerList.Loading);

            // Filter

            Filter.Update.InvokeCommand(ViewerList.Loading);
        }

        public ReactiveCommand<FootprintViewModel?, FootprintViewModel?> ClickOnItem { get; }

        public void SelectFootprintInfo(string name)
        {
            ViewerList.SelectItem(name);
        }

        public FootprintViewModel? GetFootprintViewModel(string name)
        {
            return ViewerList.GetItem(name);
        }

        [Reactive]
        public IFilter<FootprintViewModel> Filter { get; private set; }

        [Reactive]
        public IViewerList<FootprintViewModel> ViewerList { get; private set; }

        [Reactive]
        public bool ScrollToCenter { get; set; } = false;
    }
}
