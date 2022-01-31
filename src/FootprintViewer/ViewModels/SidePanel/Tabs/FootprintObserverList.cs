using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class FootprintObserverList : ReactiveObject
    {
        private readonly ReactiveCommand<FootprintInfo, FootprintInfo> select;
        private readonly ReactiveCommand<FootprintInfo, FootprintInfo> unselect;
        private FootprintInfo? _prevSelectedItem;

        public FootprintObserverList()
        {
            select = ReactiveCommand.Create<FootprintInfo, FootprintInfo>(s => s);
            unselect = ReactiveCommand.Create<FootprintInfo, FootprintInfo>(s => s);

            FootprintInfos = new ObservableCollection<FootprintInfo>();

            UpdateAsync = ReactiveCommand.CreateFromTask<Func<IEnumerable<FootprintInfo>>>(UpdateAsyncImpl);
        }

        public IObservable<FootprintInfo> SelectItem => select;

        public IObservable<FootprintInfo> UnselectItem => unselect;

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

        private async Task UpdateAsyncImpl(Func<IEnumerable<FootprintInfo>> load)
        {
            var footprints = await Task.Run(() =>
            {
                Thread.Sleep(500);
                return load();
            });

            FootprintInfos = new ObservableCollection<FootprintInfo>(footprints);
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

        public ReactiveCommand<Func<IEnumerable<FootprintInfo>>, Unit> UpdateAsync { get; }

        [Reactive]
        public ObservableCollection<FootprintInfo> FootprintInfos { get; private set; }
    }

    public static class FootprintObserverListExtensions
    {
        public static void InvalidateData(this FootprintObserverList list, Func<IEnumerable<Footprint>> load)
        {
            list.UpdateAsync.Execute(() => load().Select(s => new FootprintInfo(s))).Subscribe();
        }

        public static void InvalidateData(this FootprintObserverList list, Func<IEnumerable<FootprintInfo>> load)
        {
            list.UpdateAsync.Execute(() => load()).Subscribe();
        }
    }
}
