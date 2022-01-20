using FootprintViewer.Data;
using FootprintViewer.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
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
    public interface IFootprintDataSource
    {
        Task<List<Footprint>> GetFootprintsAsync();

        List<Footprint> GetFootprints();
    }

    public class FootprintObserverList : ReactiveObject
    {
        private readonly IFootprintDataSource _dataSource;
        private readonly ReactiveCommand<Unit, Unit> begin;
        private readonly ReactiveCommand<Unit, Unit> end;
     
        public FootprintObserverList(IReadonlyDependencyResolver dependencyResolver)
        {
            _dataSource = dependencyResolver.GetExistingService<IFootprintDataSource>();

            FootprintInfos = new ObservableCollection<FootprintInfo>();

            this.WhenAnyValue(s => s.SelectedFootprintInfo).WhereNotNull().Subscribe(footprint =>
            {
                CloseItems();

                footprint.IsShowInfo = true;
            });

            begin = ReactiveCommand.Create(() => { });

            end = ReactiveCommand.Create(() => { });
        }

        public IObservable<Unit> BeginUpdate => begin;

        public IObservable<Unit> EndUpdate => end;

        public IObservable<FootprintInfo?> SelectedFootprintInfoObserver => this.WhenAnyValue(s => s.SelectedFootprintInfo);

        public void Update() => FootprintsChanged();

        public void CloseItems() => FootprintInfos.ToList().ForEach(s => s.IsShowInfo = false);

        private static async Task<IList<FootprintInfo>> LoadDataAsync(IFootprintDataSource dataSource)
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(500);
                return dataSource.GetFootprints().Select(s => new FootprintInfo(s)).ToList();
            });
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

        private async void FootprintsChanged()
        {
            begin.Execute().Subscribe();

            var footprints = await LoadDataAsync(_dataSource);

            FootprintInfos = new ObservableCollection<FootprintInfo>(footprints);

            end.Execute().Subscribe();
        }

        //private async void FootprintsChanged()
        //{
        //    if (_footrpintLayer != null)
        //    {
        //        var footprints = await LoadDataAsync(_footrpintLayer);

        //        if (footprints != null)
        //        {
        //            if (Filter == null)
        //            {
        //                FootprintInfos = new ObservableCollection<FootprintInfo>(footprints.Select(s => new FootprintInfo(s)));
        //            }

        //            if (Filter != null)
        //            {
        //                var list = new List<FootprintInfo>();

        //                foreach (var item in footprints)
        //                {
        //                    if (Filter.Filtering(item) == true)
        //                    {
        //                        list.Add(new FootprintInfo(item));
        //                    }
        //                }

        //                FootprintInfos = new ObservableCollection<FootprintInfo>(list);
        //            }
        //        }
        //    }
        //}

        //[Reactive]
        //public bool ScrollToCenter { get; set; } = false;

        [Reactive]
        public FootprintInfo? SelectedFootprintInfo { get; set; }

        [Reactive]
        public ObservableCollection<FootprintInfo> FootprintInfos { get; set; }
    }
}
