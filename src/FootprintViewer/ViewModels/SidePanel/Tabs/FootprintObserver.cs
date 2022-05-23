using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class FootprintObserverList : ViewerList<FootprintInfo>
    {
        public FootprintObserverList(IProvider<FootprintInfo> provider) : base(provider)
        {

        }
    }

    public class FootprintObserver : SidePanelTab
    {
        private readonly IViewerList<FootprintInfo> _footprintObserverList;
        private readonly IFilter<FootprintInfo> _filter;

        public FootprintObserver(IReadonlyDependencyResolver dependencyResolver)
        {
            var mapNavigator = dependencyResolver.GetExistingService<IMapNavigator>();
            var footprintProvider = dependencyResolver.GetExistingService<IProvider<FootprintInfo>>();

            _filter = new FootprintObserverFilter(dependencyResolver);

            _footprintObserverList = new FootprintObserverList(footprintProvider);

            Title = "Просмотр рабочей программы";

            ClickOnItem = ReactiveCommand.Create<FootprintInfo?, FootprintInfo?>(s => { _footprintObserverList.ClickOnItem(s); return s; });

            FilterClick = ReactiveCommand.Create(FilterClickImpl);

            this.WhenAnyValue(s => s.IsActive).Where(s => s == true).Select(_ => _filter).InvokeCommand(_footprintObserverList.Loading);

            this.WhenAnyValue(s => s.IsActive).Where(active => active == false).Subscribe(_ => IsFilterOpen = false);

            _footprintObserverList.Select.Select(s => s.Center).Subscribe(coord => mapNavigator.SetFocusToCoordinate(coord.X, coord.Y));

            _filter.Update.InvokeCommand(_footprintObserverList.Loading);

            this.WhenAnyValue(s => s.IsExpanded).Where(c => c == false).Subscribe(_ => IsFilterOpen = false);

            MainContent = (FootprintObserverList)_footprintObserverList;
        }

        public ReactiveCommand<FootprintInfo?, FootprintInfo?> ClickOnItem { get; }

        public ReactiveCommand<Unit, Unit> FilterClick { get; }

        public void SelectFootprintInfo(string name)
        {
            _footprintObserverList.SelectItem(name);
        }

        public FootprintInfo? GetFootprintInfo(string name)
        {
            return _footprintObserverList.GetItem(name);
        }

        private void FilterClickImpl()
        {
            IsFilterOpen = !IsFilterOpen;
        }

        public IFilter<FootprintInfo> Filter => _filter;

        [Reactive]
        public bool IsFilterOpen { get; private set; }

        [Reactive]
        public ReactiveObject? MainContent { get; private set; }

        [Reactive]
        public bool ScrollToCenter { get; set; } = false;
    }
}
