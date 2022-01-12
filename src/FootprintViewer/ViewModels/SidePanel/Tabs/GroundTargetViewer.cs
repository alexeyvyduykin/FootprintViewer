using FootprintViewer.Layers;
using Mapsui;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Reactive;
using FootprintViewer.Data;
using FootprintViewer.Models;
using Mapsui.Layers;
using System.Linq;
using System.Threading.Tasks;
using Mapsui.Providers;
using DynamicData;
using System.Threading;
using System.Xml.Linq;

namespace FootprintViewer.ViewModels
{
    public class GroundTargetInfo : ReactiveObject
    {
        private readonly GroundTarget _groundTarget;

        public GroundTargetInfo() { }

        public GroundTargetInfo(GroundTarget groundTarget)
        {
            _groundTarget = groundTarget;              
            Type = groundTarget.Type;
            Name = groundTarget.Name;
        }

        public GroundTarget GroundTarget => _groundTarget;

        [Reactive]
        public string? Name { get; set; }

        [Reactive]
        public GroundTargetType Type { get; set; }
    }

    public enum TargetViewerContentType
    {
        Empty,
        Show,
        Update
    }

    public class GroundTargetViewer : SidePanelTab
    {
        private readonly TargetLayer _targetLayer;
        private double _resolution;
        private IEnumerable<IFeature>? _features;

        public GroundTargetViewer() { }

        public GroundTargetViewer(Map map)
        {
            GroundTargetInfos = new ObservableCollection<GroundTargetInfo>();

            _targetLayer = map.GetLayer<TargetLayer>(LayerType.GroundTarget);

            MouseOverEnterCommand = ReactiveCommand.Create<GroundTargetInfo>(ShowHighlightTarget);

            MouseOverLeaveCommand = ReactiveCommand.Create(HideHighlightTarget);

            SelectedItemChangedCommand = ReactiveCommand.Create<GroundTargetInfo>(SelectionChanged);

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

            _targetLayer.OnRefreshData += _targetLayer_OnRefreshData;
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

        private static async Task<IList<GroundTarget>> LoadDataAsync(TargetLayer layer, IEnumerable<IFeature> features)
        {
            return await Task.Run(() => 
            {
                Thread.Sleep(500);
                return layer.GetTargets(features).ToList();
            });
        }

        private async void GroundTargetsChanged()
        {
            if (_features != null)
            {
                var targets = await LoadDataAsync(_targetLayer, _features);

                //GroundTargetInfos.Clear();

                //foreach (var item in targets)
                //{
                //    GroundTargetInfos.Add(new GroundTargetInfo(item));
                //}

                GroundTargetInfos = new ObservableCollection<GroundTargetInfo>(targets.Select(s => new GroundTargetInfo(s)));

                Type = TargetViewerContentType.Show;
            }
        }


        public ReactiveCommand<GroundTargetInfo, Unit> MouseOverEnterCommand { get; }

        public ReactiveCommand<Unit, Unit> MouseOverLeaveCommand { get; }

        public ReactiveCommand<GroundTargetInfo, Unit> SelectedItemChangedCommand { get; }

        private void ShowHighlightTarget(GroundTargetInfo groundTarget)
        {
            if(groundTarget != null)
            {
                var name = groundTarget.Name;

                if (name != null)
                {
                    _targetLayer.ShowHighlight(name);
                }
            }
        }

        private void HideHighlightTarget()
        {
            _targetLayer.HideHighlight();
        }

        private void SelectionChanged(GroundTargetInfo groundTarget)
        {
            //if (groundTarget != null)
            //{
            //    var name = groundTarget.Name;

            //    if (name != null)
            //    {
            //        _targetLayer.SelectGroundTarget(name);
            //    }
            //}

            if (SelectedGroundTargetInfo != null)
            {
                var name = SelectedGroundTargetInfo.Name;
                if (name != null)
                {
                    _targetLayer.SelectGroundTarget(name);
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
