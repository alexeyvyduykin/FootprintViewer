using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class FootprintObserver : SidePanelTab
    {
        public FootprintObserver(IReadonlyDependencyResolver dependencyResolver)
        {
            var footprintProvider = dependencyResolver.GetExistingService<IProvider<Footprint>>();
            var viewModelFactory = dependencyResolver.GetExistingService<ViewModelFactory>();

            ViewerList = viewModelFactory.CreateFootprintViewerList(footprintProvider);

            Filter = new FootprintObserverFilter(dependencyResolver);

            Title = "Просмотр рабочей программы";

            ClickOnItem = ReactiveCommand.Create<FootprintInfo?, FootprintInfo?>(s => { ViewerList.ClickOnItem(s); return s; });

            // First loading

            this.WhenAnyValue(s => s.IsActive)
                .Take(1)
                .Where(active => active == true)
                .Select(_ => Filter)
                .InvokeCommand(ViewerList.Loading);

            // Filter

            Filter.Update.InvokeCommand(ViewerList.Loading);
        }

        public ReactiveCommand<FootprintInfo?, FootprintInfo?> ClickOnItem { get; }

        public void SelectFootprintInfo(string name)
        {
            ViewerList.SelectItem(name);
        }

        public FootprintInfo? GetFootprintInfo(string name)
        {
            return ViewerList.GetItem(name);
        }

        [Reactive]
        public IFilter<FootprintInfo> Filter { get; private set; }

        [Reactive]
        public IViewerList<FootprintInfo> ViewerList { get; private set; }

        [Reactive]
        public bool ScrollToCenter { get; set; } = false;
    }
}
