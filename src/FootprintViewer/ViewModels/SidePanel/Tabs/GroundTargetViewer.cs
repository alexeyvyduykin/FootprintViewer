using FootprintViewer.Data;
using FootprintViewer.Layers;
using Mapsui.Providers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public enum TargetViewerContentType
    {
        Empty,
        Show,
        Update
    }

    public interface IGroundTargetDataSource
    {
        IEnumerable<GroundTarget> GetTargets(IEnumerable<IFeature> features);

        IEnumerable<GroundTarget> GetTargets();

        void ShowHighlight(string name);

        void HideHighlight();

        void SelectGroundTarget(string name);

        event TargetLayerEventHandler OnRefreshData;
    }

    public class GroundTargetViewer : SidePanelTab
    {
        //  private readonly TargetLayer _targetLayer;
        private double _resolution;
        private IEnumerable<IFeature>? _features;
        private readonly IGroundTargetDataSource _dataSource;
        private readonly ReactiveCommand<GroundTargetInfo?, Unit> _selectedItem;

        public GroundTargetViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            _dataSource = dependencyResolver.GetExistingService<IGroundTargetDataSource>();

            GroundTargetInfos = new ObservableCollection<GroundTargetInfo>();

            Title = "Просмотр наземных целей";
            Name = "GroundTargetViewer";

            ShowHighlight = ReactiveCommand.Create<GroundTargetInfo?>(ShowHighlightImpl);

            HideHighlight = ReactiveCommand.Create(HideHighlightImpl);

            _selectedItem = ReactiveCommand.Create<GroundTargetInfo?>(SelectedItemIml);

            this.WhenAnyValue(s => s.Type).Subscribe(type =>
            {
                if (type == TargetViewerContentType.Update)
                {
                    GroundTargetsChanged();
                }
            });

            this.WhenAnyValue(s => s.IsActive).Subscribe(active =>
            {
                if (active == true)
                {
                    if (_features != null)
                    {
                        Type = TargetViewerContentType.Update;
                    }
                    else
                    {
                        Type = TargetViewerContentType.Empty;
                    }
                }
            });

            this.WhenAnyValue(s => s.SelectedGroundTargetInfo).InvokeCommand(this, s => s._selectedItem);

            _dataSource.OnRefreshData += _targetLayer_OnRefreshData;
        }

        private void _targetLayer_OnRefreshData(object? sender, TargetLayerEventArgs e)
        {
            _resolution = e.Resolution;
            _features = e.Features;

            if (IsActive == true)
            {
                if (e.Features != null)
                {
                    Type = TargetViewerContentType.Update;
                }
                else
                {
                    Type = TargetViewerContentType.Empty;
                }
            }
        }

        private static async Task<IList<GroundTarget>> LoadDataAsync(IGroundTargetDataSource dataSource, IEnumerable<IFeature> features)
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(500);
                return dataSource.GetTargets(features).ToList();
            });
        }

        private async void GroundTargetsChanged()
        {
            if (_features != null)
            {
                var targets = await LoadDataAsync(_dataSource, _features);

                //GroundTargetInfos.Clear();

                //foreach (var item in targets)
                //{
                //    GroundTargetInfos.Add(new GroundTargetInfo(item));
                //}

                GroundTargetInfos = new ObservableCollection<GroundTargetInfo>(targets.Select(s => new GroundTargetInfo(s)));

                Type = TargetViewerContentType.Show;
            }
        }

        public async void UpdateAll()
        {
            var targets = await Task.Run(() =>
            {              
                return _dataSource.GetTargets().ToList();
            });

            GroundTargetInfos = new ObservableCollection<GroundTargetInfo>(targets.Select(s => new GroundTargetInfo(s)));

            //SelectedGroundTargetInfo = GroundTargetInfos.FirstOrDefault();

            Type = TargetViewerContentType.Show;
        }

        public ReactiveCommand<GroundTargetInfo?, Unit> ShowHighlight { get; }

        public ReactiveCommand<Unit, Unit> HideHighlight { get; }
    
        private void ShowHighlightImpl(GroundTargetInfo? groundTarget)
        {
            if (groundTarget != null)
            {
                var name = groundTarget.Name;

                if (name != null)
                {
                    _dataSource.ShowHighlight(name);
                }
            }
        }

        private void HideHighlightImpl()
        {
            _dataSource.HideHighlight();
        }

        private void SelectedItemIml(GroundTargetInfo? groundTarget)
        {
            if (groundTarget != null)
            {
                var name = groundTarget.Name;
                
                if (string.IsNullOrEmpty(name) == false)
                {
                    _dataSource.SelectGroundTarget(name);
                }
            }
        }

        [Reactive]
        public TargetViewerContentType Type { get; set; } = TargetViewerContentType.Empty;

        [Reactive]
        public GroundTargetInfo? SelectedGroundTargetInfo { get; set; }

        [Reactive]
        public ObservableCollection<GroundTargetInfo> GroundTargetInfos { get; set; }
    }
}
