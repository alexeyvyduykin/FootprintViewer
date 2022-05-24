using FootprintViewer.Data;
using FootprintViewer.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class GroundTargetViewerList : ViewerList<GroundTargetInfo>
    {
        public GroundTargetViewerList(IProvider<GroundTargetInfo> provider) : base(provider)
        {

        }
    }

    public class GroundTargetViewer : SidePanelTab
    {
        private readonly ReactiveCommand<GroundTargetInfo?, Unit> _selectedItem;
        private readonly ITargetLayerSource _source;
        private readonly IProvider<GroundTargetInfo> _groundTargetProvider;

        public GroundTargetViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            _groundTargetProvider = dependencyResolver.GetExistingService<IProvider<GroundTargetInfo>>();
            _source = dependencyResolver.GetExistingService<ITargetLayerSource>();

            Title = "Просмотр наземных целей";

            Preview = new PreviewMainContent("Наземные цели при текущем приблежение не доступны");

            ShowHighlight = ReactiveCommand.Create<GroundTargetInfo?>(ShowHighlightImpl);

            HideHighlight = ReactiveCommand.Create(HideHighlightImpl);

            _selectedItem = ReactiveCommand.Create<GroundTargetInfo?>(SelectedItemIml);

            ViewerList = new GroundTargetViewerList(_groundTargetProvider);

            ViewerList.SelectedItemObservable.Select(s => s).InvokeCommand(_selectedItem);

            this.WhenAnyValue(s => s.IsActive).Where(s => s == true).Select(_ => Unit.Default).InvokeCommand(_source.Refresh);

            _source.Refresh.Where(_ => IsActive == true).Subscribe(names => IsEnable = !(names == null));

            _source.Refresh.Where(_ => IsActive == true && IsEnable == true).Subscribe(names => ViewerList.FiringUpdate(names));
        }

        public async Task<List<GroundTargetInfo>> GetGroundTargetInfoAsync(string name)
        {
            return await _groundTargetProvider.GetValuesAsync(new NameFilter<GroundTargetInfo>(new[] { name }));
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
        public bool IsEnable { get; set; }

        [Reactive]
        public PreviewMainContent Preview { get; private set; }

        [Reactive]
        public IViewerList<GroundTargetInfo> ViewerList { get; private set; }
    }
}
