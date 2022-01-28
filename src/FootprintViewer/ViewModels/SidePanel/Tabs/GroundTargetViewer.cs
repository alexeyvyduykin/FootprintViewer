using FootprintViewer.Data;
using FootprintViewer.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class GroundTargetViewer : SidePanelTab
    {
        private readonly TargetLayer _targetLayer;
        private readonly GroundTargetViewerList _groundTargetViewerList;
        private readonly PreviewMainContent _emptyMainContent;
        private readonly PreviewMainContent _updateMainContent;
        private readonly ReactiveCommand<GroundTargetInfo?, Unit> _selectedItem;

        public GroundTargetViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            //_targetLayer = dependencyResolver.GetExistingService<TargetLayer>();

            Title = "Просмотр наземных целей";

            Name = "GroundTargetViewer";

            _groundTargetViewerList = new GroundTargetViewerList();

            _emptyMainContent = new PreviewMainContent("Наземные цели при текущем приблежение не доступны");

            _updateMainContent = new PreviewMainContent("Загрузка...");

            ShowHighlight = ReactiveCommand.Create<GroundTargetInfo?>(ShowHighlightImpl);

            HideHighlight = ReactiveCommand.Create(HideHighlightImpl);

            _selectedItem = ReactiveCommand.Create<GroundTargetInfo?>(SelectedItemIml);

            //this.WhenAnyValue(s => s.IsActive).Where(s => s == true).Subscribe(_ => GroundTargetsChanged());

            _groundTargetViewerList.SelectedItemObservable.InvokeCommand(_selectedItem);

            _groundTargetViewerList.UpdateAsync.IsExecuting.Subscribe(isExecuting =>
            {
                if (isExecuting == true)
                {
                    MainContent = _updateMainContent;
                }
                else
                {
                    MainContent = _groundTargetViewerList;
                }
            });

            //_targetLayer.IsEnabledObserver.Subscribe(isEnabled =>
            //{
            //    if (isEnabled == true)
            //    {
            //        GroundTargetsChanged();
            //    }
            //    else
            //    {
            //        MainContent = _emptyMainContent;
            //    }
            //});
        }

        private void GroundTargetsChanged()
        {
            if (_targetLayer.IsEnable == true)
            {
                _groundTargetViewerList.InvalidateData(() => _targetLayer.GetTargets());
            }
        }

        public void UpdateAsync(Func<IEnumerable<GroundTargetInfo>> load) => _groundTargetViewerList.InvalidateData(load);

        public void UpdateAsync(Func<IEnumerable<GroundTarget>> load) => _groundTargetViewerList.InvalidateData(load);

        private void SelectedItemIml(GroundTargetInfo? groundTarget)
        {
            if (groundTarget != null)
            {
                var name = groundTarget.Name;

                if (string.IsNullOrEmpty(name) == false)
                {
                    _targetLayer.SelectGroundTarget(name);
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
                    _targetLayer.ShowHighlight(name);
                }
            }
        }

        private void HideHighlightImpl()
        {
            _targetLayer.HideHighlight();
        }

        public ReactiveCommand<GroundTargetInfo?, Unit> ShowHighlight { get; }

        public ReactiveCommand<Unit, Unit> HideHighlight { get; }

        [Reactive]
        public ReactiveObject? MainContent { get; set; }
    }
}
