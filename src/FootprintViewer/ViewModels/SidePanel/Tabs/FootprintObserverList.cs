using FootprintViewer.Data;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class FootprintObserverList : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<List<FootprintInfo>> _footprintInfos;
        private readonly ObservableAsPropertyHelper<bool> _isLoading;
        private readonly ReactiveCommand<FootprintInfo, FootprintInfo> select;
        private readonly ReactiveCommand<FootprintInfo, FootprintInfo> unselect;
        private FootprintInfo? _prevSelectedItem;
        private readonly FootprintProvider _footprintProvider;

        public FootprintObserverList(FootprintProvider provider)
        {
            _footprintProvider = provider;

            Loading = ReactiveCommand.CreateFromTask<FootprintObserverFilter?, List<FootprintInfo>>(LoadingAsync);

            select = ReactiveCommand.Create<FootprintInfo, FootprintInfo>(s => s);

            unselect = ReactiveCommand.Create<FootprintInfo, FootprintInfo>(s => s);

            _footprintInfos = Loading.ToProperty(this, x => x.FootprintInfos, scheduler: RxApp.MainThreadScheduler);

            _isLoading = Loading.IsExecuting.ToProperty(this, x => x.IsLoading, scheduler: RxApp.MainThreadScheduler);
        }

        private async Task<List<FootprintInfo>> LoadingAsync(FootprintObserverFilter? filter = null)
        {
            if (filter == null)
            {
                return await _footprintProvider.GetFootprintInfosAsync();
            }
            else
            {
                return await Task.Run(() =>
                {
                    var list = _footprintProvider.GetFootprintInfosAsync().Result;

                    return list.Where(s => filter.Filtering(s)).ToList();
                });
            }
        }

        public ReactiveCommand<FootprintObserverFilter?, List<FootprintInfo>> Loading { get; }

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

        public void SelectFootprintInfo(string name)
        {
            var item = FootprintInfos.Where(s => s.Name.Equals(name)).FirstOrDefault();

            ClickOnItem(item);
        }

        public FootprintInfo? GetFootprintInfo(string name)
        {
            return FootprintInfos.Where(s => s.Name.Equals(name)).FirstOrDefault();
        }

        //private void ScrollCollectionToCenter(FootprintInfo item)
        //{
        //    ScrollToCenter = true;

        //    SelectedFootprintInfo = item;

        //    ScrollToCenter = false;
        //}

        //[Reactive]
        //public bool ScrollToCenter { get; set; } = false;

        public List<FootprintInfo> FootprintInfos => _footprintInfos.Value;

        public bool IsLoading => _isLoading.Value;
    }
}
