using FootprintViewer.Layers;
using Mapsui.Projection;
using Mapsui;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using FootprintViewer.Data;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Splat;

namespace FootprintViewer.ViewModels
{
    public interface IFootprintDataSource
    {
        Task<List<Footprint>> GetFootprintsAsync();
    }

    public class FootprintObserverList : ReactiveObject
    {  
        private readonly IFootprintDataSource? _dataSource;

        private readonly ObservableAsPropertyHelper<List<FootprintInfo>> _footprints;

        public FootprintObserverList(IReadonlyDependencyResolver dependencyResolver)
        {
            _dataSource = dependencyResolver.GetService<IFootprintDataSource>();
         
            PreviewMouseLeftButtonDownCommand = ReactiveCommand.Create(PreviewMouseLeftButtonDown);

            ClickOnItemCommand = ReactiveCommand.Create<FootprintInfo>(ClickOnItem);

            LoadFootprints = ReactiveCommand.CreateFromTask(_dataSource.GetFootprintsAsync);

            this.WhenAnyValue(s => s.SelectedFootprintInfo).Subscribe(footprint =>
            {
                if (footprint != null)
                {
                    FootprintInfos.ToList().ForEach(s => s.IsShowInfo = false);

                    footprint.IsShowInfo = true;
                }
            });

            _footprints = LoadFootprints.Throttle(TimeSpan.FromSeconds(2)).Select(s => s.Select(t => new FootprintInfo(t)).ToList()).ToProperty(this, x => x.FootprintInfos, scheduler: RxApp.MainThreadScheduler);
        }

        public IObservable<bool> PreviewMouseLeftButtonCommandCheckerObserver => this.WhenAnyValue(s => s.PreviewMouseLeftButtonCommandChecker);

        public IObservable<FootprintInfo?> SelectedFootprintInfoObserver => this.WhenAnyValue(s => s.SelectedFootprintInfo);

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

        private void ScrollCollectionToCenter(FootprintInfo item)
        {
            ScrollToCenter = true;

            SelectedFootprintInfo = item;

            ScrollToCenter = false;
        }

        private void PreviewMouseLeftButtonDown()
        {
            if (SelectedFootprintInfo != null)
            {
                if (SelectedFootprintInfo.IsShowInfo == true)
                {
                    SelectedFootprintInfo.IsShowInfo = false;
                }
                else
                {
                    SelectedFootprintInfo.IsShowInfo = true;
                }

                PreviewMouseLeftButtonCommandChecker = !PreviewMouseLeftButtonCommandChecker;
            }
        }

        private void ClickOnItem(FootprintInfo item)
        {
            if (SelectedFootprintInfo != null)
            {
                if (SelectedFootprintInfo.IsShowInfo == true)
                {
                    SelectedFootprintInfo.IsShowInfo = false;
                }
                else
                {
                    SelectedFootprintInfo.IsShowInfo = true;
                }

                PreviewMouseLeftButtonCommandChecker = !PreviewMouseLeftButtonCommandChecker;
            }

            SelectedFootprintInfo = item;
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

        public ReactiveCommand<Unit, Unit> PreviewMouseLeftButtonDownCommand { get; }

        public ReactiveCommand<FootprintInfo, Unit> ClickOnItemCommand { get; }

        public ReactiveCommand<Unit, List<Footprint>> LoadFootprints { get; }

        [Reactive]
        public FootprintInfo? SelectedFootprintInfo { get; set; }
      
        public List<FootprintInfo> FootprintInfos => _footprints.Value;

        [Reactive]
        public bool ScrollToCenter { get; set; } = false;

        [Reactive]
        private bool PreviewMouseLeftButtonCommandChecker { get; set; } = false;
    }
}
