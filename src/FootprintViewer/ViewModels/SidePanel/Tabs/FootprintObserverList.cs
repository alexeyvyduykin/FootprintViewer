using FootprintViewer.Layers;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class FootprintObserverList : ReactiveObject
    {
        private readonly FootprintLayer _footprintLayer;
        private readonly ReactiveCommand<Unit, Unit> begin;
        private readonly ReactiveCommand<Unit, Unit> end;
        private readonly ReactiveCommand<FootprintInfo, FootprintInfo> select;
        private readonly ReactiveCommand<FootprintInfo, FootprintInfo> unselect;
        private FootprintInfo? _prevSelectedItem;
        private readonly ObservableAsPropertyHelper<List<FootprintInfo>> _footprintInfos;

        public FootprintObserverList(IReadonlyDependencyResolver dependencyResolver)
        {
            _footprintLayer = dependencyResolver.GetExistingService<FootprintLayer>();

            begin = ReactiveCommand.Create(() => { });
            end = ReactiveCommand.Create(() => { });
            select = ReactiveCommand.Create<FootprintInfo, FootprintInfo>(s => s);
            unselect = ReactiveCommand.Create<FootprintInfo, FootprintInfo>(s => s);

            Update = ReactiveCommand.CreateFromTask<FootprintObserverFilter?, List<FootprintInfo>>(FootprintsChangedAsync);

            _footprintInfos = Update.ToProperty(this, x => x.FootprintInfos, scheduler: RxApp.MainThreadScheduler);
        }

        public IObservable<Unit> BeginUpdate => begin;

        public IObservable<Unit> EndUpdate => end;

        public IObservable<FootprintInfo> SelectItem => select;

        public IObservable<FootprintInfo> UnselectItem => unselect;

        public ReactiveCommand<FootprintObserverFilter?, List<FootprintInfo>> Update { get; }

        public void CloseItems() => FootprintInfos.ToList().ForEach(s => s.IsShowInfo = false);

        public void ClickOnItem(FootprintInfo? item)
        {
            if (item == null)
            {
                return;
            }

            if (_prevSelectedItem != null && _prevSelectedItem != item)
            {
                if (_prevSelectedItem.IsShowInfo == true)
                {
                    _prevSelectedItem.IsShowInfo = false;
                }
            }

            item.IsShowInfo = !item.IsShowInfo;

            if (item.IsShowInfo == true)
            {
                select.Execute(item).Subscribe();
            }
            else
            {
                unselect.Execute(item).Subscribe();
            }

            _prevSelectedItem = item;
        }

        private static async Task<List<FootprintInfo>> LoadDataAsync(FootprintLayer dataSource, FootprintObserverFilter? filter = null)
        {
            return await Task.Run(() =>
            {
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();

                if (filter != null)
                {
                    return dataSource.GetFootprints().Select(s => new FootprintInfo(s)).Where(f => filter.Filtering(f) == true).ToList();
                }

                return dataSource.GetFootprints().Select(s => new FootprintInfo(s)).ToList();
            });
        }

        private async Task<List<FootprintInfo>> FootprintsChangedAsync(FootprintObserverFilter? filter)
        {
            begin.Execute().Subscribe();

            var footprints = await LoadDataAsync(_footprintLayer, filter);

            end.Execute().Subscribe();

            return footprints;
        }

        //public void SelectFootprintInfo(string name)
        //{
        //    if (_footrpintLayer != null)
        //    {
        //        var isSelect = _footrpintLayer.IsSelect(name);

        //        if (isSelect == true)
        //        {
        //            _footrpintLayer.UnselectFeature(name);

        //            FootprintInfos.ToList().ForEach(s => s.IsShowInfo = false);

        //            SelectedFootprintInfo = null;
        //        }
        //        else
        //        {
        //            _footrpintLayer.SelectFeature(name);

        //            var item = FootprintInfos.Where(s => name.Equals(s.Name)).SingleOrDefault();

        //            if (item != null)
        //            {
        //                ScrollCollectionToCenter(item);
        //            }
        //        }
        //    }
        //}

        //private void ScrollCollectionToCenter(FootprintInfo item)
        //{
        //    ScrollToCenter = true;

        //    SelectedFootprintInfo = item;

        //    ScrollToCenter = false;
        //}

        //[Reactive]
        //public bool ScrollToCenter { get; set; } = false;

        public List<FootprintInfo> FootprintInfos => _footprintInfos.Value;
    }
}
