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
        private readonly GroundTargetViewerList _groundTargetViewerList;
        private readonly PreviewMainContent _previewContent;
        private readonly ReactiveCommand<GroundTargetInfo?, Unit> _selectedItem;
        private readonly ITargetLayerSource _source;

        public GroundTargetViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            var groundTargetProvider = dependencyResolver.GetExistingService<GroundTargetProvider>();
            _source = dependencyResolver.GetExistingService<ITargetLayerSource>();

            Title = "Просмотр наземных целей";

            _previewContent = new PreviewMainContent("Наземные цели при текущем приблежение не доступны");

            ShowHighlight = ReactiveCommand.Create<GroundTargetInfo?>(ShowHighlightImpl);

            HideHighlight = ReactiveCommand.Create(HideHighlightImpl);

            _selectedItem = ReactiveCommand.Create<GroundTargetInfo?>(SelectedItemIml);

            _groundTargetViewerList = new GroundTargetViewerList(groundTargetProvider);

            _groundTargetViewerList.SelectedItemObservable.InvokeCommand(_selectedItem);

            this.WhenAnyValue(s => s.IsActive).Where(s => s == true).Select(_ => Unit.Default).InvokeCommand(_source.Refresh);

            _source.Refresh.Where(_ => IsActive == true).Subscribe(names =>
            {
                IsEnable = !(names == null);

                if (IsEnable == true)
                {
                    MainContent = _groundTargetViewerList;
                }
                else
                {
                    MainContent = _previewContent;
                }
            });

            _source.Refresh.Where(_ => IsActive == true && IsEnable == true).Subscribe(names =>
            {
                _groundTargetViewerList.Update(names);
            });

            MainContent = _previewContent;
        }

        private void SelectedItemIml(GroundTargetInfo? groundTarget)
        {
            if (groundTarget != null)
            {
                var name = groundTarget.Name;

                if (string.IsNullOrEmpty(name) == false)
                {
                    _source.SelectGroundTarget(name);
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
                    _source.ShowHighlight(name);
                }
            }
        }

        private void HideHighlightImpl()
        {
            _source.HideHighlight();
        }

        public ReactiveCommand<GroundTargetInfo?, Unit> ShowHighlight { get; }

        public ReactiveCommand<Unit, Unit> HideHighlight { get; }

        [Reactive]
        public ReactiveObject MainContent { get; set; }

        private bool IsEnable { get; set; }
    }
}
