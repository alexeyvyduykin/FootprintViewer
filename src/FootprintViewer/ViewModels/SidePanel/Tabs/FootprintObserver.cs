using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class FootprintObserver : SidePanelTab
    {
        private bool _firstLoading = true;

        public FootprintObserver(IReadonlyDependencyResolver dependencyResolver)
        {
            //          var mapNavigator = dependencyResolver.GetExistingService<IMapNavigator>();
            var footprintProvider = dependencyResolver.GetExistingService<IProvider<FootprintInfo>>();

            ViewerList = ViewerListBuilder.CreateViewerList(footprintProvider);

            Filter = new FootprintObserverFilter(dependencyResolver);

            Title = "Просмотр рабочей программы";

            ClickOnItem = ReactiveCommand.Create<FootprintInfo?, FootprintInfo?>(s => { ViewerList.ClickOnItem(s); return s; });

            FilterClick = ReactiveCommand.Create(FilterClickImpl);

            // First loading

            this.WhenAnyValue(s => s.IsActive)
                .Where(active => active == true && _firstLoading == true)
                .Select(_ => Unit.Default)
                .InvokeCommand(Filter.Init);

            Filter.Init.Select(_ => Filter).InvokeCommand(ViewerList.Loading);
            Filter.Init.Subscribe(_ => _firstLoading = false);



            //          this.WhenAnyValue(s => s.IsActive).Where(active => active == false).Subscribe(_ => IsFilterOpen = false);

            //          ViewerList.Select.Select(s => s.Center).Subscribe(coord => mapNavigator.SetFocusToCoordinate(coord.X, coord.Y));

            //          _filter.Update.InvokeCommand(ViewerList.Loading);

            //          this.WhenAnyValue(s => s.IsExpanded).Where(c => c == false).Subscribe(_ => IsFilterOpen = false);
        }

        public ReactiveCommand<FootprintInfo?, FootprintInfo?> ClickOnItem { get; }

        public ReactiveCommand<Unit, Unit> FilterClick { get; }

        public void SelectFootprintInfo(string name)
        {
            ViewerList.SelectItem(name);
        }

        public FootprintInfo? GetFootprintInfo(string name)
        {
            return ViewerList.GetItem(name);
        }

        private void FilterClickImpl()
        {
            IsFilterOpen = !IsFilterOpen;
        }

        [Reactive]
        public IFilter<FootprintInfo> Filter { get; private set; }

        [Reactive]
        public bool IsFilterOpen { get; private set; }

        [Reactive]
        public IViewerList<FootprintInfo> ViewerList { get; private set; }

        [Reactive]
        public bool ScrollToCenter { get; set; } = false;
    }
}
