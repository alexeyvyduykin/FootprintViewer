using FootprintViewer.Data;
using Mapsui.Providers;
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
    public interface IGroundTargetDataSource
    {
        IEnumerable<GroundTarget> GetTargets(IEnumerable<IFeature> features);

        IEnumerable<GroundTarget> GetTargets();

        void ShowHighlight(string name);

        void HideHighlight();

        void SelectGroundTarget(string name);

        IObservable<IEnumerable<IFeature>?> RefreshDataObservable { get; }
    }

    public class GroundTargetViewer : SidePanelTab
    {
        private readonly IGroundTargetDataSource _dataSource;
        private readonly GroundTargetViewerList _groundTargetViewerList;
        private readonly PreviewMainContent _emptyMainContent;
        private readonly PreviewMainContent _updateMainContent;
        private readonly ReactiveCommand<GroundTargetInfo?, Unit> _selectedItem;

        public GroundTargetViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            _dataSource = dependencyResolver.GetExistingService<IGroundTargetDataSource>();

            Title = "Просмотр наземных целей";

            Name = "GroundTargetViewer";

            _groundTargetViewerList = new GroundTargetViewerList();

            _emptyMainContent = new PreviewMainContent("Наземные цели при текущем приблежение не доступны");

            _updateMainContent = new PreviewMainContent("Загрузка...");

            ShowHighlight = ReactiveCommand.Create<GroundTargetInfo?>(ShowHighlightImpl);

            HideHighlight = ReactiveCommand.Create(HideHighlightImpl);

            _selectedItem = ReactiveCommand.Create<GroundTargetInfo?>(SelectedItemIml);

            this.WhenAnyValue(s => s.IsActive).Where(s => s == true).Subscribe(_ => GroundTargetsChanged(_features)/* _groundTargetViewerList.Update()*/);

            _groundTargetViewerList.SelectedItemObservable.InvokeCommand(_selectedItem);

            //  _groundTargetViewerList.BeginUpdate.Subscribe(_ => MainContent = _updateMainContent);

            //  _groundTargetViewerList.EndUpdate.Subscribe(_ => MainContent = _groundTargetViewerList);

            _groundTargetViewerList.UpdateAsync.IsExecuting.Where(s => s == true).Subscribe(_ => MainContent = _updateMainContent);
            _groundTargetViewerList.UpdateAsync.IsExecuting.Where(s => s == false).Subscribe(_ => MainContent = _groundTargetViewerList);


            //   _groundTargetViewerList.Disable.Subscribe(_ => MainContent = _emptyMainContent);

            _dataSource.RefreshDataObservable.Subscribe(f => GroundTargetsChanged(f));
        }

        private IEnumerable<IFeature>? _features;

        private void GroundTargetsChanged(IEnumerable<IFeature>? features)
        {
            _features = features;

            if (features != null)
            {
                _groundTargetViewerList.InvalidateData(() => _dataSource.GetTargets(features));
            }
            else
            {
                MainContent = _emptyMainContent;
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
                    _dataSource.SelectGroundTarget(name);
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
                    _dataSource.ShowHighlight(name);
                }
            }
        }

        private void HideHighlightImpl()
        {
            _dataSource.HideHighlight();
        }

        public ReactiveCommand<GroundTargetInfo?, Unit> ShowHighlight { get; }

        public ReactiveCommand<Unit, Unit> HideHighlight { get; }

        [Reactive]
        public ReactiveObject? MainContent { get; set; }
    }
}
