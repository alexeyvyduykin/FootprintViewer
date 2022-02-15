using FootprintViewer.Data;
using FootprintViewer.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class GroundTargetViewer : SidePanelTab
    {
        private readonly TargetLayer? _targetLayer;
        private readonly GroundTargetViewerList _groundTargetViewerList;
        private readonly ReactiveCommand<GroundTargetInfo?, Unit> _selectedItem;

        public GroundTargetViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            var map = dependencyResolver.GetExistingService<Mapsui.Map>();

            var groundTargetProvider = dependencyResolver.GetExistingService<GroundTargetProvider>();

            _targetLayer = map.GetLayer<TargetLayer>(LayerType.GroundTarget);

            Title = "Просмотр наземных целей";

            Name = "GroundTargetViewer";

            PreviewContent = new PreviewMainContent("Наземные цели при текущем приблежение не доступны");

            ShowHighlight = ReactiveCommand.Create<GroundTargetInfo?>(ShowHighlightImpl);

            HideHighlight = ReactiveCommand.Create(HideHighlightImpl);

            _selectedItem = ReactiveCommand.Create<GroundTargetInfo?>(SelectedItemIml);

            _groundTargetViewerList = new GroundTargetViewerList(groundTargetProvider);

            MainContent = _groundTargetViewerList;

            _groundTargetViewerList.SelectedItemObservable.InvokeCommand(_selectedItem);

            if (_targetLayer != null)
            {
                this.WhenAnyValue(s => s.IsActive).Where(s => s == true).Select(_ => Unit.Default).InvokeCommand(_targetLayer.Refresh);

                _targetLayer.Refresh.Where(_ => IsActive == true).Subscribe(names =>
                {
                    IsEnable = !(names == null);
                });

                _targetLayer.Refresh.Where(_ => IsActive == true && IsEnable == true).Subscribe(names =>
                {
                    _groundTargetViewerList.Update(names);
                });
            }
        }

        private void SelectedItemIml(GroundTargetInfo? groundTarget)
        {
            if (groundTarget != null)
            {
                var name = groundTarget.Name;

                if (string.IsNullOrEmpty(name) == false)
                {
                    _targetLayer?.SelectGroundTarget(name);
                }
            }
        }

        private void ShowHighlightImpl(GroundTargetInfo? groundTarget)
        {
            if (groundTarget != null)
            {
                var name = groundTarget.Name;

                if (name != null)
                {
                    _targetLayer?.ShowHighlight(name);
                }
            }
        }

        private void HideHighlightImpl()
        {
            _targetLayer?.HideHighlight();
        }

        public ReactiveCommand<GroundTargetInfo?, Unit> ShowHighlight { get; }

        public ReactiveCommand<Unit, Unit> HideHighlight { get; }

        public ReactiveObject MainContent { get; }

        public ReactiveObject PreviewContent { get; }

        [Reactive]
        public bool IsEnable { get; set; }
    }
}
